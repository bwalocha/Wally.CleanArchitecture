using Microsoft.Extensions.DependencyInjection;

namespace Wally.CleanArchitecture.MicroService.Application.DI.Microsoft.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddApplication(this IServiceCollection services)
	{
		services.AddMapper();

		return services;
	}

	// public static IApplicationBuilder UseApplication(this IApplicationBuilder app, IWebHostEnvironment env)
	// {
	// 	return app;
	// }
}
