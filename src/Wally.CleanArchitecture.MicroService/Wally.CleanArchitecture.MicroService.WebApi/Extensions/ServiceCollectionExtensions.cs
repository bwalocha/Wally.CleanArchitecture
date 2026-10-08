using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Models;
using Wally.CleanArchitecture.MicroService.WebApi.Blazor.Extensions;

namespace Wally.CleanArchitecture.MicroService.WebApi.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
	{
		var settings = new AppSettings();
		configuration.Bind(settings);

		services.AddMapper(settings)
			.AddWebApi()
			.AddBlazorUi();

		return services;
	}

	public static IApplicationBuilder UsePresentation(this IApplicationBuilder app)
	{
		app.UseWebApi()
			.UseBlazorUi();

		return app;
	}
}
