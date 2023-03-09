using AutoMapper.Extensions.ExpressionMapping;

using Microsoft.Extensions.DependencyInjection;

using Wally.CleanArchitecture.MicroService.MapperProfiles;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Extensions;

public static class AutoMapperExtensions
{
	public static IServiceCollection AddMapper(this IServiceCollection services)
	{
		services.AddAutoMapper(
			cfg => { cfg.AddExpressionMapping(); },
			typeof(IApplicationMapperProfilesAssemblyMarker).Assembly);

		return services;
	}
}
