using Microsoft.AspNetCore.Builder;
// using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Wally.CleanArchitecture.MicroService.WebApi.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddPresentation(this IServiceCollection services)
	{
		services.AddMapper();
		services.AddWebApi();

		return services;
	}

	public static IApplicationBuilder UsePresentation(this IApplicationBuilder app/*, IWebHostEnvironment env*/)
	{
		app.UseWebApi();
		
		return app;
	}
}
