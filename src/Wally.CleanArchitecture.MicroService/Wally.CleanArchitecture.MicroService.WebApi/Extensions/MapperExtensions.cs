// using AutoMapper.Extensions.ExpressionMapping;
using Microsoft.Extensions.DependencyInjection;
using Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Models;

namespace Wally.CleanArchitecture.MicroService.WebApi.Extensions;

public static class MapperExtensions
{
	public static IServiceCollection AddMapper(this IServiceCollection services, AppSettings settings)
	{
		services.AddAutoMapper(a =>
		{
			// a.AddExpressionMapping();
			a.AddMaps(typeof(IPresentationAssemblyMarker).Assembly);
			a.LicenseKey = settings.MapperSettings.LicenseKey;
		});

		return services;
	}
}
