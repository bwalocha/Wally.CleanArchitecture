using AutoMapper.Extensions.ExpressionMapping;
using Microsoft.Extensions.DependencyInjection;

namespace Wally.CleanArchitecture.MicroService.WebApi.Extensions;

public static class MapperExtensions
{
	public static IServiceCollection AddMapper(this IServiceCollection services)
	{
		services.AddAutoMapper(a =>
		{
			a.AddExpressionMapping();
			a.AddMaps(typeof(IPresentationAssemblyMarker).Assembly);
		});

		return services;
	}
}
