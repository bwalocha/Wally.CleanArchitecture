using System;
using EntityFramework.Exceptions.MySQL.Pomelo;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Wally.CleanArchitecture.MicroService.Application.Abstractions;
using Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Models;
using Wally.CleanArchitecture.MicroService.Infrastructure.Persistence;
using Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.MySql;
using Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.PostgreSQL;
using Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.SQLite;
using Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.SqlServer;
using ExceptionProcessorExtensions = EntityFramework.Exceptions.PostgreSQL.ExceptionProcessorExtensions;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Extensions;

public static class PersistenceExtensions
{
	public static IServiceCollection AddPersistence(this IServiceCollection services, AppSettings settings)
	{
		void DbContextOptions(DbContextOptionsBuilder options)
		{
			switch (settings.Database.ProviderType)
			{
				case DatabaseProviderType.None:
					return;
				case DatabaseProviderType.InMemory:
					WithInMemory(options);
					break;
				case DatabaseProviderType.MySql:
					WithMySql(options, settings);
					break;
				case DatabaseProviderType.PostgreSQL:
					WithNpgsql(options, settings);
					break;
				case DatabaseProviderType.SQLite:
					WithSqlite(options, settings);
					break;
				case DatabaseProviderType.SqlServer:
					WithSqlServer(options, settings);
					break;
				default:
					throw new NotSupportedException(
						$"Not supported Database Provider type: '{settings.Database.ProviderType}'");
			}

			options.EnableSensitiveDataLogging(); // TODO: Use env.IsDevelopment
			options.ConfigureWarnings(builder =>
			{
				builder.Default(WarningBehavior.Throw);
				builder.Ignore(InMemoryEventId.TransactionIgnoredWarning);
				builder.Ignore(RelationalEventId.MultipleCollectionIncludeWarning);
				builder.Ignore(SqlServerEventId.SavepointsDisabledBecauseOfMARS);
				builder.Log(CoreEventId.SensitiveDataLoggingEnabledWarning);
			});
#if DEBUG
			options.EnableSensitiveDataLogging();
#endif
		}

		services.AddDbContext<DbContext, ApplicationDbContext>((Action<DbContextOptionsBuilder>)DbContextOptions);

		services.Scan(
			a => a.FromAssemblyOf<IInfrastructurePersistenceAssemblyMarker>()
				.AddClasses(c => c.AssignableTo(typeof(IReadOnlyRepository<,>)))
				.AsImplementedInterfaces()
				.WithScopedLifetime());

		services.AddSingleton(TimeProvider.System);

		return services;
	}

	private static void WithInMemory(DbContextOptionsBuilder options)
	{
		options.UseInMemoryDatabase(nameof(DatabaseProviderType.InMemory), builder => builder.EnableNullChecks());
	}

	private static void WithMySql(DbContextOptionsBuilder options, AppSettings settings)
	{
		options.UseMySql(
				settings.ConnectionStrings.Database,
				MySqlServerVersion.LatestSupportedServerVersion,
				builder =>
				{
					builder.MigrationsHistoryTable(HistoryRepository.DefaultTableName, ApplicationDbContext.SchemaName);
					builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
					builder.MigrationsAssembly(
						typeof(IInfrastructureMySqlAssemblyMarker).Assembly.GetName()
							.Name);
				})
			.UseExceptionProcessor();
	}

	private static void WithNpgsql(DbContextOptionsBuilder options, AppSettings settings)
	{
		options.UseNpgsql(
			settings.ConnectionStrings.Database,
			builder =>
			{
				builder.MigrationsHistoryTable(HistoryRepository.DefaultTableName, ApplicationDbContext.SchemaName);
				builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
				builder.MigrationsAssembly(
					typeof(IInfrastructurePostgreSqlAssemblyMarker).Assembly.GetName()
						.Name);
			});
		ExceptionProcessorExtensions.UseExceptionProcessor(options);
	}

	private static void WithSqlite(DbContextOptionsBuilder options, AppSettings settings)
	{
		options.UseSqlite(
				settings.ConnectionStrings.Database,
				builder =>
				{
					/*
					Unable to create a 'DbContext' of type 'ApplicationDbContext'.
					The exception 'An error was generated for warning 'Microsoft.EntityFrameworkCore.Model.Validation.SchemaConfiguredWarning':
					The entity type 'User' is configured to use schema 'MicroService', but SQLite does not support schemas.
					This configuration will be ignored by the SQLite provider.
					This exception can be suppressed or logged by passing event ID 'SqliteEventId.SchemaConfiguredWarning' to the 'ConfigureWarnings' method in 'DbContext.OnConfiguring' or 'AddDbContext'.' was thrown while attempting to create an instance. For the different patterns supported at design time, see https://go.microsoft.com/fwlink/?linkid=851728
					*/
					builder.MigrationsHistoryTable(HistoryRepository.DefaultTableName, ApplicationDbContext.SchemaName);
					builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
					builder.MigrationsAssembly(
						typeof(IInfrastructureSQLiteAssemblyMarker).Assembly.GetName()
							.Name);
				})
			.ConfigureWarnings(
				builder => { builder.Ignore(SqliteEventId.SchemaConfiguredWarning); });
		EntityFramework.Exceptions.Sqlite.ExceptionProcessorExtensions.UseExceptionProcessor(options);
	}

	private static void WithSqlServer(DbContextOptionsBuilder options, AppSettings settings)
	{
		options.UseSqlServer(
			settings.ConnectionStrings.Database,
			builder =>
			{
				builder.MigrationsHistoryTable(
					HistoryRepository.DefaultTableName,
					ApplicationDbContext.SchemaName);
				builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
				builder.MigrationsAssembly(
					typeof(IInfrastructureSqlServerAssemblyMarker).Assembly.GetName()
						.Name);
			});
		EntityFramework.Exceptions.SqlServer.ExceptionProcessorExtensions.UseExceptionProcessor(options);
	}

	public static IApplicationBuilder UsePersistence(this IApplicationBuilder app)
	{
		var settings = app.ApplicationServices.GetRequiredService<IOptions<AppSettings>>();

		if (!settings.Value.Database.IsMigrationEnabled ||
			settings.Value.Database.ProviderType == DatabaseProviderType.None ||
			settings.Value.Database.ProviderType == DatabaseProviderType.InMemory)
		{
			return app;
		}

		using var scope = app.ApplicationServices.CreateScope();
		var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();
		dbContext.Database.Migrate();

		return app;
	}
}
