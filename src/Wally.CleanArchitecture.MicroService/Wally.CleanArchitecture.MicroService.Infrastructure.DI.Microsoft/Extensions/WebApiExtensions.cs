using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Wally.CleanArchitecture.MicroService.Application;
using Wally.CleanArchitecture.MicroService.Application.Abstractions;
using Wally.CleanArchitecture.MicroService.Application.Contracts;
// using Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Filters;
using Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Handlers;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Extensions;

public static class WebApiExtensions
{
	public static IServiceCollection AddWebApi(this IServiceCollection services)
	{
		services.AddProblemDetails(options =>
		{
			options.CustomizeProblemDetails = a =>
			{
				a.ProblemDetails.Extensions.TryAdd("traceId", a.HttpContext.TraceIdentifier);
				// a.ProblemDetails.Extensions.TryAdd("correlationId", options.);
			};
		});
		
		services.AddExceptionHandler<AuthenticationExceptionHandler>();
		services.AddExceptionHandler<AuthorizationExceptionHandler>();
		services.AddExceptionHandler<ValidationExceptionHandler>();
		services.AddExceptionHandler<NotFoundExceptionHandler>();
		services.AddExceptionHandler<DatabaseExceptionHandler>();
		services.AddExceptionHandler<HttpGlobalExceptionHandler>();
		
		services.AddControllers(/*settings => { settings.Filters.Add(typeof(HttpGlobalExceptionFilter)); }*/)
			.AddOData(
				options =>
				{
					options.Filter()
						.OrderBy()
						.Count()
						.SetMaxTop(1000);
				})
			.AddNewtonsoftJson(options =>
			{
				options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
			});

		services.AddValidatorsFromAssemblyContaining<IApplicationAssemblyMarker>();
		services.AddValidatorsFromAssemblyContaining<IApplicationContractsAssemblyMarker>();
		services.AddFluentValidationAutoValidation(config => config.DisableDataAnnotationsValidation = true);
		services.Configure<ApiBehaviorOptions>(options =>
		{
			options.InvalidModelStateResponseFactory = context => throw new ValidationException(context
				.ModelState
				.Where(a => a.Value?.ValidationState == ModelValidationState.Invalid)
				.Select(a => new ValidationFailure(a.Key,
					string.Join(", ", a.Value!.Errors.Select(b => b.ErrorMessage)), a.Value.AttemptedValue)));
		});
		
		services.AddHttpContextAccessor();
		services.AddScoped<IRequestContext, RequestContext>();

		return services;
	}

	public static IApplicationBuilder UseWebApi(this IApplicationBuilder app)
	{
		// app.UseExceptionHandler();
		app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
		app.UseStatusCodePages(async statusCodeContext =>
		{
			statusCodeContext.HttpContext.Response.ContentType = MediaTypeNames.Text.Plain;

			await statusCodeContext.HttpContext.Response.WriteAsync(
				$"Status Code Page: {statusCodeContext.HttpContext.Response.StatusCode}");
		});

		return app;
	}
}
