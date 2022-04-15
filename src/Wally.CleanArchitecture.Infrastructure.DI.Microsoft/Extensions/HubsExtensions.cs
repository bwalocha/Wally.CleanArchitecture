using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace Wally.CleanArchitecture.Infrastructure.DI.Microsoft.Extensions;

public static class HubsExtensions
{
	public static IServiceCollection AddEventHub(this IServiceCollection services)
	{
		services.AddSignalR(options => { options.EnableDetailedErrors = true; });

		return services;
	}

	public static IApplicationBuilder UseEventHub<THub>(this IApplicationBuilder app) where THub : Hub
	{
		app.UseDefaultFiles();
		app.UseStaticFiles();

		app.UseEndpoints(endpoints => { endpoints.MapHub<THub>("/hub"); });

		return app;
	}
}
