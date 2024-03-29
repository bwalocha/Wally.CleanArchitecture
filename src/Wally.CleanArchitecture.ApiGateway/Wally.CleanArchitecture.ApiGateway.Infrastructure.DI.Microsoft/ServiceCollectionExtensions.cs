using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using Wally.CleanArchitecture.ApiGateway.Infrastructure.DI.Microsoft.Extensions;
using Wally.CleanArchitecture.ApiGateway.Infrastructure.DI.Microsoft.Models;

namespace Wally.CleanArchitecture.ApiGateway.Infrastructure.DI.Microsoft;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
	{
		var settings = new AppSettings();
		configuration.Bind(settings);

		services.AddFeatureManagement();
		services.AddOptions(settings);
		services.AddReverseProxy(configuration);
		services.AddHealthChecks(settings);
		services.AddApiCors(settings.Cors);
		services.AddOpenApi(Assembly.GetCallingAssembly(), settings);

		return services;
	}

	public static IApplicationBuilder UseInfrastructure(
		this IApplicationBuilder app, 
		IWebHostEnvironment env,
		IOptions<AppSettings> options,
		IFeatureManager featureManager)
	{
		// Configure the HTTP request pipeline.
		if (env.IsDevelopment())
		{
			app.UseDeveloperExceptionPage();
		}

		if (!featureManager.IsEnabled(FeatureFlags.SwaggerDisabled))
		{
			app.UseOpenApi(options.Value.SwaggerAuthentication,
				options.Value.ReverseProxy);
		}

		// app.UseHttpsRedirection(); // TODO: App is hosted by Docker, HTTPS is not required inside container

		app.UseRouting();
		app.UseApiCors();

		// app.UseAuthentication(); // TODO: configure Auth2

		app.UseHealthChecks();
		app.UseReverseProxy();

		return app;
	}
}
