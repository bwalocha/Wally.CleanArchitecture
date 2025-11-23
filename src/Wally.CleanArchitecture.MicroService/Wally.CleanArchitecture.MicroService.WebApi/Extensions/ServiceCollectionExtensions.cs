using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Models;

namespace Wally.CleanArchitecture.MicroService.WebApi.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
	{
		var settings = new AppSettings();
		configuration.Bind(settings);

		services.AddMapper(settings);
		services.AddWebApi();

		return services;
	}

	public static IApplicationBuilder UsePresentation(this IApplicationBuilder app)
	{
		app.UseWebApi();

		return app;
	}
}
