using System;

using HealthChecks.UI.Client;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Wally.CleanArchitecture.WebApi.Extensions;

public static class HealthChecksExtensions
{
	public static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddHealthChecks()
			.AddSqlServer(
				configuration.GetConnectionString(Constants.Database),
				name: "DB",
				failureStatus: HealthStatus.Degraded,
				tags: new[] { "DB", "Database", "MSSQL", })
			.AddRabbitMQ(
				new Uri(configuration.GetConnectionString(Constants.ServiceBus)),
				name: "MQ",
				failureStatus: HealthStatus.Degraded,
				tags: new[] { "MQ", "Messaging", "ServiceBus", });
		services.AddHealthChecksUI()
			.AddInMemoryStorage();

		return services;
	}

	public static IApplicationBuilder UseHealthChecks(this IApplicationBuilder app)
	{
		app.UseHealthChecks(
			"/healthChecks",
			new HealthCheckOptions
			{
				Predicate = _ => true, ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
			});
		app.UseHealthChecksUI();

		return app;
	}
}
