using AutoMapper.Extensions.ExpressionMapping;
using Microsoft.Extensions.DependencyInjection;
using Wally.CleanArchitecture.MicroService.Application.MapperProfiles;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Extensions;

public static class MapperExtensions
{
	public static IServiceCollection AddMapper(this IServiceCollection services)
	{
		services.AddAutoMapper(a =>
		{
			a.AddExpressionMapping();
			a.AddMaps(typeof(IApplicationMapperProfilesAssemblyMarker).Assembly);
		});

		return services;
	}
}
