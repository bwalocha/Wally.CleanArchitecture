// using AutoMapper.Extensions.ExpressionMapping;

using Mapster;
using Microsoft.Extensions.DependencyInjection;
using Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Models;

namespace Wally.CleanArchitecture.MicroService.WebApi.Extensions;

public static class MapperExtensions
{
	public static IServiceCollection AddMapper(this IServiceCollection services, AppSettings settings)
	{
		// AutoMapper
		services.AddAutoMapper(a =>
		{
			// a.AddExpressionMapping();
			a.AddMaps(typeof(Wally.CleanArchitecture.MicroService.WebApi.Mapper.AutoMapper.IPresentationMapperAssemblyMarker).Assembly);
			a.LicenseKey = settings.MapperSettings.LicenseKey;
		});
		
		// Mapster
		var config = new TypeAdapterConfig();
		config.Default.IgnoreNullValues(true);
		config.Scan(typeof(Wally.CleanArchitecture.MicroService.WebApi.Mapper.Mapster.IPresentationMapperAssemblyMarker).Assembly);
		config.Compile();

		services.AddSingleton(config);
		services.AddScoped<MapsterMapper.IMapper, MapsterMapper.Mapper>();

		return services;
	}
}
