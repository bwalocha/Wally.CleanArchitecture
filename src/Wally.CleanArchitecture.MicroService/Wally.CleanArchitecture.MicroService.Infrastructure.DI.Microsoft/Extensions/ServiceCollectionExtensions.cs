using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Hubs;
using Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Models;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Extensions;

// TODO: move to Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.Extensions
public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
	{
		var settings = new AppSettings();
		configuration.Bind(settings);

		services.AddOptions(settings);
		services.AddWebApi();
		services.AddCqrs();
		services.AddOpenApi(Assembly.GetCallingAssembly());
		services.AddHealthChecks(settings);
		services.AddPersistence(settings);
		services.AddMessaging(settings);
		services.AddEventHub();
		services.AddBackgroundServices(settings);

		return services;
	}

	public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app, IWebHostEnvironment env)
	{
		// app.UseExceptionHandler();

		// Configure the HTTP request pipeline.
		if (env.IsDevelopment())
		{
//			app.UseDeveloperExceptionPage();
			// app.UseSwagger();
		}

		app.UseOpenApi();

		// If the App is hosted by Docker, HTTPS is not required inside container
		// app.UseHttpsRedirection();

		// app.UseRouting();
		
		// app.UseAuthentication(); // TODO: Consider only for ApiGateway
		app.UseAuthorization();
		app.UseHealthChecks();

		app.UseWebApi();

		app.UsePersistence();
		app.UseEventHub<EventHub>();

		return app;
	}
}
