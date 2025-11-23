using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Wally.CleanArchitecture.MicroService.Application;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Extensions;

public static class WebApiExtensions
{
	public static IServiceCollection AddWebApi(this IServiceCollection services)
	{
		services.AddValidatorsFromAssemblyContaining<IApplicationAssemblyMarker>(includeInternalTypes: true)
			// 	.AddValidatorsFromAssemblyContaining<IApplicationContractsAssemblyMarker>(includeInternalTypes: true)
			// 	.AddFluentValidationAutoValidation(config => config.DisableDataAnnotationsValidation = true)
			// 	.Configure<ApiBehaviorOptions>(options =>
			// 	{
			// 		options.InvalidModelStateResponseFactory = context => throw new ValidationException(context
			// 			.ModelState
			// 			.Where(a => a.Value?.ValidationState == ModelValidationState.Invalid)
			// 			.Select(a => new ValidationFailure(a.Key,
			// 				string.Join(", ", a.Value!.Errors.Select(b => b.ErrorMessage)), a.Value.AttemptedValue)));
			// 	})
			;

		// services.AddHttpContextAccessor();
		// services.AddScoped<IRequestContext, RequestContext>();

		return services;
	}

	public static IApplicationBuilder UseWebApi(this IApplicationBuilder app)
	{
		return app;
	}
}
