using System;
using System.Threading;
using System.Threading.Tasks;

using HealthChecks.UI.Client;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Extensions;

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
				tags: new[] { "MQ", "Messaging", "ServiceBus", })
			.AddVersionHealthCheck();
		/*
		services.AddHealthChecksUI() // TODO: Consider only for ApiGateway
			.AddInMemoryStorage();
		*/

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
		/*
		app.UseHealthChecksUI(); // TODO: Consider only for ApiGateway
		*/

		return app;
	}
	
	private static IHealthChecksBuilder AddVersionHealthCheck(this IHealthChecksBuilder builder)
	{
		builder.AddCheck<VersionHealthCheck>("VER", tags: new[] { "VER", "Version" });
		
		return builder;
	}
	
	private class VersionHealthCheck : IHealthCheck
	{
		private readonly string? _version;

		public VersionHealthCheck()
		{
			_version = GetType().Assembly.GetName().Version?.ToString();
		}

		public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken)
		{
			return Task.FromResult(HealthCheckResult.Healthy(_version));
		}
	}
}
