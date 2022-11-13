using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Wally.CleanArchitecture.ApiGateway.WebApi.Extensions;

public static class ReverseProxyExtensions
{
	private const string ReverseProxy = nameof(ReverseProxy);

	public static IServiceCollection AddReverseProxy(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddReverseProxy()
			.LoadFromConfig(configuration.GetSection(ReverseProxy));

		return services;
	}

	public static IApplicationBuilder UseReverseProxy(this IApplicationBuilder app)
	{
		app.UseEndpoints(endpoints => { endpoints.MapReverseProxy(); });

		return app;
	}
}
