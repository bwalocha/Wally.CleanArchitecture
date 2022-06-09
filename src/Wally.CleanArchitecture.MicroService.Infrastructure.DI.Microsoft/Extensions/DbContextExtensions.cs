using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

using Wally.CleanArchitecture.MicroService.Domain.Abstractions;
using Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Models;
using Wally.CleanArchitecture.MicroService.Persistence;
using Wally.CleanArchitecture.MicroService.Persistence.SqlServer;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Extensions;

public static class DbContextExtensions
{
	public static IServiceCollection AddDbContext(this IServiceCollection services, AppSettings settings)
	{
		Action<DbContextOptionsBuilder> dbContextOptions;
		dbContextOptions = options =>
		{
			options.UseSqlServer(
				settings.ConnectionStrings.Database,
				builder =>
				{
					builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
					builder.MigrationsAssembly(
						typeof(Helper).Assembly.GetName()
							.Name);
				});

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

		return services;
	}

	public static IApplicationBuilder UseDbContext(
		this IApplicationBuilder app,
		DbContext dbContext,
		DbContextSettings settings)
	{
		if (settings.IsMigrationEnabled)
		{
			dbContext.Database.Migrate();
		}

		return app;
	}
}
