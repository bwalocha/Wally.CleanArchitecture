using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

using Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Extensions;
using Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Models;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddInfrastructure(this IServiceCollection services, AppSettings settings)
	{
		services.AddCqrs();
		services.AddSwagger(Assembly.GetCallingAssembly());
		services.AddHealthChecks(settings);
		services.AddDbContext(settings);
		services.AddMapper();
		services.AddMessaging(settings);
		services.AddEventHub();

		return services;
	}
}
