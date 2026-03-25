using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace Wally.CleanArchitecture.MicroService.Application.DI.Microsoft.Extensions;

public static class MapperExtensions
{
	public static IServiceCollection AddMapper(this IServiceCollection services/*, AppSettings settings*/)
	{
		// AutoMapper
		services.AddAutoMapper(a =>
		{
			a.AddMaps(typeof(Mapper.AutoMapper.IApplicationMapperAssemblyMarker).Assembly);
			// a.LicenseKey = settings.MapperSettings.LicenseKey;
		});
		services.AddScoped<Wally.CleanArchitecture.MicroService.Application.Abstractions.IMapper, Wally.CleanArchitecture.MicroService.Application.Mapper.AutoMapper.MapperAdapter>();
		
		// Mapster
		var config = new TypeAdapterConfig();
		config.Default.IgnoreNullValues(true);
		config.Scan(typeof(Wally.CleanArchitecture.MicroService.Application.Mapper.Mapster.IApplicationMapperAssemblyMarker).Assembly);
		config.Compile();

		services.AddSingleton(config);
		services.AddScoped<MapsterMapper.IMapper, MapsterMapper.Mapper>();
		// services.AddScoped<Wally.CleanArchitecture.MicroService.Application.Abstractions.IMapper, Wally.CleanArchitecture.MicroService.Application.Mapper.Mapster.MapperAdapter>();

		return services;
	}
}
