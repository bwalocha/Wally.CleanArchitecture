using System.Linq;
using System.Reflection;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

using Wally.CleanArchitecture.ApiGateway.Infrastructure.DI.Microsoft.Models;

namespace Wally.CleanArchitecture.ApiGateway.Infrastructure.DI.Microsoft.Extensions;

public static class SwaggerExtensions
{
	public static IServiceCollection AddSwagger(
		this IServiceCollection services,
		Assembly assembly,
		AppSettings settings)
	{
		return services;
	}

	public static IApplicationBuilder UseSwagger(
		this IApplicationBuilder app,
		AuthenticationSettings settings,
		ReverseProxySettings reverseProxySettings)
	{
		app.UseSwaggerUI(
			options =>
			{
				options.OAuthClientId(settings.ClientId);
				options.OAuthClientSecret(settings.ClientSecret);
				options.OAuthUsePkce();
				options.DefaultModelsExpandDepth(0);

				options.RoutePrefix = "swagger";
				foreach (var route in reverseProxySettings.Routes)
				{
					foreach (var prefix in route.Transforms!.SelectMany(a => a.Values))
					{
						options.SwaggerEndpoint(
							$"{prefix}/swagger/v1/swagger.json",
							$"Wally.CleanArchitecture API [{route.ClusterId} - *]");
					}

					var cluster = reverseProxySettings.Clusters[route.ClusterId];
					foreach (var destination in cluster.Destinations!.Values)
					{
						options.SwaggerEndpoint(
							$"{destination.Address}/swagger/v1/swagger.json",
							$"Wally.CleanArchitecture API [{route.ClusterId} - {destination.Address}]");
					}
				}
			});

		return app;
	}
}
