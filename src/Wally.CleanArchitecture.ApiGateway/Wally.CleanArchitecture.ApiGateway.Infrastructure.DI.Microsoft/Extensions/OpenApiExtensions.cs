using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Wally.CleanArchitecture.ApiGateway.Infrastructure.DI.Microsoft.Models;

namespace Wally.CleanArchitecture.ApiGateway.Infrastructure.DI.Microsoft.Extensions;

public static class OpenApiExtensions
{
	public static IServiceCollection AddOpenApi(this IServiceCollection services, Assembly assembly, AppSettings settings)
	{
		return services;
	}

	public static IApplicationBuilder UseOpenApi(this IApplicationBuilder app, AuthenticationSettings settings, ReverseProxySettings reverseProxySettings)
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
					foreach (var prefix in route.Transforms!.SelectMany(a => a.Values)
								.Distinct())
					{
						var url = $"https://localhost:5001{prefix}/swagger/v1/swagger.json";
						var name = $"Wally.CleanArchitecture API [{route.ClusterId}]";

						options.SwaggerEndpoint(url, name);
					}
				}

				options.ConfigObject.Urls = options.ConfigObject.Urls.DistinctBy(a => a.Url);
			});

		return app;
	}
}
