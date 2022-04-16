using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Extensions;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddInfrastructure(IServiceCollection services, IConfiguration configuration)
	{
		services.AddDbContext(configuration);

		return services;
	}
}
