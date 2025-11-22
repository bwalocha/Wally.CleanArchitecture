using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using TickerQ.Dashboard.DependencyInjection;
using TickerQ.DependencyInjection;
using TickerQ.EntityFrameworkCore.DbContextFactory;
using TickerQ.EntityFrameworkCore.DependencyInjection;

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
				schedulerOptions.NodeIdentifier = "notification-server";
			});

			// options.SetExceptionHandler<NotificationExceptionHandler>();

			options.AddOperationalStore(efOptions =>
			{
				efOptions.UseTickerQDbContext<TickerQDbContext>(optionsBuilder =>
				{
					// optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("TickerQConnection"), 
					// 	cfg =>
					// 	{
					// 		cfg.EnableRetryOnFailure(3, TimeSpan.FromSeconds(5));
					// 	});
				});
				efOptions.SetDbContextPoolSize(34);
			});

			// Dashboard
			options.AddDashboard(dashboardOptions =>
			{
				dashboardOptions.SetBasePath("/scheduler");
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
