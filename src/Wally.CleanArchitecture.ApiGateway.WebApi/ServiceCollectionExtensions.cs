using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Wally.CleanArchitecture.ApiGateway.WebApi.Extensions;
using Wally.CleanArchitecture.ApiGateway.WebApi.Models;

namespace Wally.CleanArchitecture.ApiGateway.WebApi;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddInfrastructure(
		this IServiceCollection services,
		IConfiguration configuration,
		AppSettings settings)
	{
		services.AddReverseProxy(configuration);
		services.AddHealthChecks(settings);
		services.AddApiCors(settings.Cors);

		return services;
	}
}
