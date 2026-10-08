// using Microsoft.AspNetCore.Builder;
// using Microsoft.Extensions.DependencyInjection;
// using Wally.CleanArchitecture.MicroService.WebApi.Blazor.Services;

namespace Wally.CleanArchitecture.MicroService.WebApi.Blazor.Extensions;

public static class BlazorUiExtensions
{
	public static IServiceCollection AddBlazorUi(this IServiceCollection services)
	{
		services.AddRazorComponents()
			.AddInteractiveServerComponents();
		// services.AddScoped<RequestActivityTracker>();
		// services.AddScoped<Application.Abstractions.IRequestActivityTracker>(
		// 	services => services.GetRequiredService<RequestActivityTracker>());

		// Aggregate pages call ISender directly (registered by the Application layer's DI extensions), so no
		// per-aggregate api client or HttpClient registration is needed here anymore.

		return services;
	}

	public static IApplicationBuilder UseBlazorUi(this IApplicationBuilder app)
	{
		// .NET 10 serves framework assets (e.g. _framework/blazor.web.js) through the endpoint-routing based
		// static assets pipeline. UseStaticFiles() alone does not resolve them when the Blazor UI lives in a
		// referenced Razor Class Library, so MapStaticAssets() is required in addition to it.
		app.UseStaticFiles();
		app.UseEndpoints(endpoints =>
		{
			endpoints.MapStaticAssets();
			endpoints.MapRazorComponents<Components.App>()
				.AddInteractiveServerRenderMode();
		});

		return app;
	}
}
