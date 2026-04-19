using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wally.CleanArchitecture.ApiGateway.Infrastructure.DI.Microsoft.Models;

namespace Wally.CleanArchitecture.ApiGateway.WebApi.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
	{
		var settings = new AppSettings();
		configuration.Bind(settings);

		services.AddWebApi();

		return services;
	}

	public static IApplicationBuilder UsePresentation(this IApplicationBuilder app)
	{
		app.UseWebApi();

		return app;
	}
}
