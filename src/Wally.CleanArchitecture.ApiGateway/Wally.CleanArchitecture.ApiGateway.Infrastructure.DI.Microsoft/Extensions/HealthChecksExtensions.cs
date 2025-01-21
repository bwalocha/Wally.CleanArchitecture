using System.Threading;
using System.Threading.Tasks;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Wally.CleanArchitecture.ApiGateway.Infrastructure.DI.Microsoft.Models;

namespace Wally.CleanArchitecture.ApiGateway.Infrastructure.DI.Microsoft.Extensions;

public static class HealthChecksExtensions
{
	// Adds Readiness
	public static IServiceCollection AddHealthChecks(this IServiceCollection services, AppSettings settings)
	{
		services.AddHealthChecks()
			.AddVersionHealthCheck();
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
				Predicate = _ => true,
				ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
			});

		app.UseHealthChecksUI();

		app.UseEndpoints(
			endpoints =>
			{
				// Adds Liveness
				endpoints.MapGet(
					"/",
					async context =>
					{
						await context.Response.WriteAsync(
							$"v{typeof(HealthChecksExtensions).Assembly.GetName().Version}",
							context.RequestAborted);
					});
			});

		return app;
	}

	private static IHealthChecksBuilder AddVersionHealthCheck(this IHealthChecksBuilder builder)
	{
		builder.AddCheck<VersionHealthCheck>(
			"VER",
			tags: new[]
			{
				"VER", "Version",
			});

		return builder;
	}

	private sealed class VersionHealthCheck : IHealthCheck
	{
		private readonly string? _version;

#pragma warning disable S1144
		public VersionHealthCheck()
		{
			_version = GetType()
				.Assembly.GetName()
				.Version?.ToString();
		}
#pragma warning restore S1144

		public Task<HealthCheckResult> CheckHealthAsync(
			HealthCheckContext context,
			CancellationToken cancellationToken = default)
		{
			return Task.FromResult(HealthCheckResult.Healthy(_version));
		}
	}
}
