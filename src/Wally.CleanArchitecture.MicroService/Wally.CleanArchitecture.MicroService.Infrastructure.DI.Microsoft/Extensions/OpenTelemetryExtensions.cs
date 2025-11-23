using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Models;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Extensions;

public static class OpenTelemetryExtensions
{
	// https://www.youtube.com/watch?v=nFU-hcHyl2s
	public static IServiceCollection AddOpenTelemetry(this IServiceCollection services, OpenTelemetrySettings settings)
	{
		services.AddOpenTelemetry()
			.ConfigureResource(resource => resource.AddService("Wally.CleanArchitecture.MicroService", 
				typeof(IInfrastructureDIMicrosoftAssemblyMarker)
				.Assembly.GetName()
				.Version?.ToString()))
			.WithMetrics(metrics =>
			{
				metrics
					.AddRuntimeInstrumentation()
					.AddAspNetCoreInstrumentation()
					.AddHttpClientInstrumentation();

				metrics.AddOtlpExporter(options =>
				{
					if (settings.Endpoint != null)
					{
						options.Endpoint = settings.Endpoint;
					}
				});

				metrics.AddMeter(
						"Microsoft.AspNetCore.Hosting",
						"Microsoft.AspNetCore.Server.Kestrel",
						"System.Net.Http",
						"Wally.CleanArchitecture.MicroService.WebApi");

				metrics.AddPrometheusExporter();
			})
			.WithTracing(tracing =>
			{
#if DEBUG
				tracing.SetSampler<AlwaysOnSampler>();
#endif

				tracing.AddAspNetCoreInstrumentation()
					.AddHttpClientInstrumentation();
				// .AddEntityFrameworkCoreInstrumentation();

				tracing.AddOtlpExporter();
			});

		// builder.Logging.AddOpenTelemetry(logging => logging.AddOtlpExplerer());

		services.Configure<OpenTelemetryLoggerOptions>(a => a.AddOtlpExporter());
		services.ConfigureOpenTelemetryLoggerProvider(a => a.AddOtlpExporter());
		services.ConfigureOpenTelemetryMeterProvider(a => a.AddOtlpExporter());
		services.ConfigureOpenTelemetryTracerProvider(a => a.AddOtlpExporter());

		return services;
	}

	public static IApplicationBuilder UseOpenTelemetry(this IApplicationBuilder app)
	{
		app.UseOpenTelemetryPrometheusScrapingEndpoint();
		
		return app;
	}
}
