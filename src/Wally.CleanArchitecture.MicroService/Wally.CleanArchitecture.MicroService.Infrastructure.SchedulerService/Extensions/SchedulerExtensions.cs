using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using TickerQ.Dashboard.DependencyInjection;
using TickerQ.DependencyInjection;
using TickerQ.EntityFrameworkCore.Customizer;
using TickerQ.EntityFrameworkCore.DependencyInjection;
using Wally.CleanArchitecture.MicroService.Infrastructure.Persistence;
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
				efOptions.UseApplicationDbContext<ApplicationDbContext>(ConfigurationType.IgnoreModelCustomizer);
			});

			// Dashboard
			options.AddDashboard(dashboardOptions =>
			{
				dashboardOptions.SetBasePath("/scheduler");
				dashboardOptions.WithNoAuth(); // TODO: consider dashboardOptions.WithBasicAuth(user, password);
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
