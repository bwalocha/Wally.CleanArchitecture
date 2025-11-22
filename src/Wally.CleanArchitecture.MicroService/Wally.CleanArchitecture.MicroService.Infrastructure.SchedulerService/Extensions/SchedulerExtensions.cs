using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TickerQ.Dashboard.DependencyInjection;
using TickerQ.DependencyInjection;
using TickerQ.EntityFrameworkCore.DbContextFactory;
using TickerQ.EntityFrameworkCore.DependencyInjection;
using Wally.CleanArchitecture.MicroService.Infrastructure.SchedulerService.Handlers;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.SchedulerService.Extensions;

public static class SchedulerExtensions
{
	public static IServiceCollection AddScheduler(this IServiceCollection services)
	{
		services.AddTickerQ(options =>
		{
			options.ConfigureScheduler(schedulerOptions =>
			{
				schedulerOptions.MaxConcurrency = 10;
				schedulerOptions.NodeIdentifier = "Wally.CleanArchitecture.MicroService";
			});

			// options.IgnoreSeedDefinedCronTickers(); // Disable automatic seeding
			
			options.SetExceptionHandler<JobExceptionHandler>();

			options.AddOperationalStore(efOptions =>
			{
				efOptions.UseTickerQDbContext<TickerQDbContext>(optionsBuilder =>
				{
					// optionsBuilder.UseSqlite("Data Source=Wally.CleanArchitecture.MicroService.Scheduler.db;Cache=Shared");
					optionsBuilder.UseInMemoryDatabase("Wally.CleanArchitecture.MicroService.Scheduler", builder => builder.EnableNullChecks());
				});

				// efOptions.UseApplicationDbContext<DbContext>(ConfigurationType.IgnoreModelCustomizer);
				// efOptions.SetDbContextPoolSize(34);
			});

			// Dashboard
			options.AddDashboard(dashboardOptions =>
			{
				dashboardOptions.SetBasePath("/scheduler");
				dashboardOptions.WithNoAuth();
				// dashboardOptions.WithBasicAuth("admin", "secure-password");
			});
		});

		return services;
	}

	public static IApplicationBuilder UseScheduler(this IApplicationBuilder app)
	{
		app.UseTickerQ();

		return app;
	}
}
