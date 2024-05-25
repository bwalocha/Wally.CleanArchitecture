using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Wally.CleanArchitecture.ApiGateway.Infrastructure.DI.Microsoft.Models;

namespace Wally.CleanArchitecture.ApiGateway.Infrastructure.DI.Microsoft.Extensions;

public static class OpenTelemetryExtensions
{
	public static IServiceCollection AddOpenTelemetry(this IServiceCollection services, OpenTelemetrySettings settings)
	{
		services.AddOpenTelemetry()
			.ConfigureResource(resource => resource.AddService("Wally.CleanArchitecture"))
			.WithMetrics(metrics =>
			{
				metrics.AddAspNetCoreInstrumentation()
					.AddHttpClientInstrumentation();
				
				metrics.AddOtlpExporter(options =>
				{
					if (settings.Endpoint != null)
					{
						options.Endpoint = settings.Endpoint;
					}
				});
			})
			.WithTracing(tracing =>
			{
				tracing.AddAspNetCoreInstrumentation()
					.AddHttpClientInstrumentation();
				// .AddEntityFrameworkCoreInstrumentation();
				
				tracing.AddOtlpExporter();
			});
		
		// builder.Logging.AddOpenTelemetry(logging => logging.AddOtlpExplerer());
		
		return services;
	}
	
	public static IApplicationBuilder UseOpenTelemetry(this IApplicationBuilder app)
	{
		return app;
	}
}
