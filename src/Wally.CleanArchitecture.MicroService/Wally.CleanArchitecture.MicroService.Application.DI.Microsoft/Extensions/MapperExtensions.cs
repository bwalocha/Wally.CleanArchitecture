// using AutoMapper.Extensions.ExpressionMapping;
using Microsoft.Extensions.DependencyInjection;
using Wally.CleanArchitecture.MicroService.Application.MapperProfiles;

namespace Wally.CleanArchitecture.MicroService.Application.DI.Microsoft.Extensions;

public static class MapperExtensions
{
	public static IServiceCollection AddMapper(this IServiceCollection services/*, AppSettings settings*/)
	{
		services.AddAutoMapper(a =>
		{
			// a.AddExpressionMapping();
			a.AddMaps(typeof(IApplicationMapperProfilesAssemblyMarker).Assembly);
			// a.LicenseKey = settings.MapperSettings.LicenseKey;
		});

		return services;
	}
}
