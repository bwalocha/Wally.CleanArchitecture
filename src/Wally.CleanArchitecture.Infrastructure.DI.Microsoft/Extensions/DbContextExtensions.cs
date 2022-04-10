﻿using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Wally.CleanArchitecture.Domain.Abstractions;
using Wally.CleanArchitecture.Infrastructure.DI.Microsoft.Models;
using Wally.CleanArchitecture.Persistence;
using Wally.CleanArchitecture.Persistence.SqlServer.Migrations;

namespace Wally.CleanArchitecture.Infrastructure.DI.Microsoft.Extensions;

public static class DbContextExtensions
{
	public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
	{
		Action<DbContextOptionsBuilder> dbContextOptions;
		dbContextOptions = options =>
		{
			options.UseSqlServer(
				configuration.GetConnectionString(Constants.Database),
				builder =>
				{
					builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
					builder.MigrationsAssembly(
						typeof(Initial).Assembly.GetName()
							.Name);
				});

			options.ConfigureWarnings(
				builder =>
				{
					builder.Default(WarningBehavior.Throw);
					builder.Ignore(RelationalEventId.MultipleCollectionIncludeWarning);
					builder.Ignore(SqlServerEventId.SavepointsDisabledBecauseOfMARS);
				});
		};
		services.AddDbContext<ApplicationDbContext>(dbContextOptions);

		services.Scan(
			a => a.FromApplicationDependencies(b => b.FullName!.StartsWith("Wally.CleanArchitecture."))
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
