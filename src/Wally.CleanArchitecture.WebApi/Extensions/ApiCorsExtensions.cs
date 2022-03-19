using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

using Wally.CleanArchitecture.WebApi.Models;

namespace Wally.CleanArchitecture.WebApi.Extensions;

public static class ApiCorsExtensions
{
	private const string CorsPolicy = nameof(CorsPolicy);

	public static IServiceCollection AddApiCors(this IServiceCollection services, AppSettings appSettings)
	{
		services.AddCors(
			options => options.AddPolicy(
				CorsPolicy,
				builder => builder.WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS")
					.AllowAnyHeader()
					.AllowCredentials()
					.WithOrigins(appSettings.Cors.Origins.ToArray())));

		return services;
	}

	public static IApplicationBuilder UseApiCors(this IApplicationBuilder app)
	{
		app.UseCors(CorsPolicy);

		return app;
	}
}
