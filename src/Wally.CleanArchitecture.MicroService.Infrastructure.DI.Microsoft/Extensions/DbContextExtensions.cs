using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Wally.CleanArchitecture.MicroService.Domain.Abstractions;
using Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Models;
using Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Providers;
using Wally.CleanArchitecture.MicroService.Infrastructure.Persistence;
using Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.MySql;
using Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.PostgreSQL;
using Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.SQLite;
using Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.SqlServer;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Extensions;

public static class DbContextExtensions
{
	public static IServiceCollection AddDbContext(this IServiceCollection services, AppSettings settings)
	{
		Action<DbContextOptionsBuilder> dbContextOptions;
		dbContextOptions = options =>
		{
			switch (settings.Database.ProviderType)
			{
				case DatabaseProviderType.MySql:
					options.UseMySql(
						settings.ConnectionStrings.Database,
						MySqlServerVersion.LatestSupportedServerVersion,
						builder =>
						{
							builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
							builder.MigrationsAssembly(
								typeof(IInfrastructureMySqlAssemblyMarker).Assembly.GetName()
									.Name);
						});
					EntityFramework.Exceptions.MySQL.Pomelo.ExceptionProcessorExtensions.UseExceptionProcessor(options);
					break;
				case DatabaseProviderType.PostgreSQL:
					options.UseNpgsql(
						settings.ConnectionStrings.Database,
						builder =>
						{
							builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
							builder.MigrationsAssembly(
								typeof(IInfrastructurePostgreSQLAssemblyMarker).Assembly.GetName()
									.Name);
						});
					EntityFramework.Exceptions.PostgreSQL.ExceptionProcessorExtensions.UseExceptionProcessor(options);
					break;
				case DatabaseProviderType.SQLite:
					options.UseSqlite(
						settings.ConnectionStrings.Database,
						builder =>
						{
							builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
							builder.MigrationsAssembly(
								typeof(IInfrastructureSQLiteAssemblyMarker).Assembly.GetName()
									.Name);
						});
					EntityFramework.Exceptions.Sqlite.ExceptionProcessorExtensions.UseExceptionProcessor(options);
					break;
				case DatabaseProviderType.SqlServer:
					options.UseSqlServer(
						settings.ConnectionStrings.Database,
						builder =>
						{
							builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
							builder.MigrationsAssembly(
								typeof(IInfrastructureSqlServerAssemblyMarker).Assembly.GetName()
									.Name);
						});
					EntityFramework.Exceptions.SqlServer.ExceptionProcessorExtensions.UseExceptionProcessor(options);
					break;
				default:
					throw new ArgumentOutOfRangeException(
						nameof(settings.Database.ProviderType),
						"Unknown Database Provider Type");
			}

			options.ConfigureWarnings(
				builder =>
				{
					builder.Default(WarningBehavior.Throw);
					builder.Ignore(RelationalEventId.MultipleCollectionIncludeWarning);
					builder.Ignore(SqlServerEventId.SavepointsDisabledBecauseOfMARS);
					builder.Log(CoreEventId.SensitiveDataLoggingEnabledWarning);
				});

			options.EnableSensitiveDataLogging(); // TODO: get from configuration
		};
		services.AddDbContext<DbContext, ApplicationDbContext>(dbContextOptions);

		services.Scan(
			a => a.FromApplicationDependencies(b => b.FullName!.StartsWith("Wally.CleanArchitecture.MicroService."))
				.AddClasses(c => c.AssignableTo(typeof(IReadOnlyRepository<>)))
				.AsImplementedInterfaces()
				.WithScopedLifetime());

		services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
		services.AddScoped<IUserProvider, HttpUserProvider>();

		return services;
	}

	public static IApplicationBuilder UseDbContext(this IApplicationBuilder app)
	{
		var settings = app.ApplicationServices.GetRequiredService<IOptions<AppSettings>>();

		if (!settings.Value.Database.IsMigrationEnabled)
		{
			return app;
		}

		var dbContext = app.ApplicationServices.GetRequiredService<DbContext>();
		dbContext.Database.Migrate();

		return app;
	}
}
