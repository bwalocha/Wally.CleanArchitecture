using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Extensions;

public static class HubsExtensions
{
	public static IServiceCollection AddEventHub(this IServiceCollection services)
	{
		services.AddSignalR(options => { options.EnableDetailedErrors = true; });

		return services;
	}

	public static IApplicationBuilder UseEventHub<THub>(this IApplicationBuilder app)
		where THub : Hub
	{
		app.UseDefaultFiles();
		app.UseStaticFiles();

		app.UseEndpoints(endpoints =>
		{
			// TODO: Use type name
			endpoints.MapHub<THub>("/hub", options =>
			{
				options.Transports =
					HttpTransportType.WebSockets |
					HttpTransportType.LongPolling;
				options.CloseOnAuthenticationExpiration = true;
				options.ApplicationMaxBufferSize = 65_536;
				options.TransportMaxBufferSize = 65_536;
				options.MinimumProtocolVersion = 0;
				options.TransportSendTimeout = TimeSpan.FromSeconds(10);
				options.WebSockets.CloseTimeout = TimeSpan.FromSeconds(3);
				options.LongPolling.PollTimeout = TimeSpan.FromSeconds(10);
			});
		});
		// app.UseEndpoints(endpoints => { endpoints.MapHub<THub>("/hub"); });

		return app;
	}
}
