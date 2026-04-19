using System.Collections.Generic;
using System.Linq;
// using System.Net.Mime;
using System.Reflection;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Wally.CleanArchitecture.ApiGateway.WebApi.Contracts;

// using Newtonsoft.Json;

namespace Wally.CleanArchitecture.ApiGateway.WebApi.Extensions;

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
		// services.AddExceptionHandler<AuthenticationExceptionHandler>()
		// 	.AddExceptionHandler<AuthorizationExceptionHandler>()
		// 	.AddExceptionHandler<ValidationExceptionHandler>()
		// 	.AddExceptionHandler<NotFoundExceptionHandler>()
		// 	.AddExceptionHandler<DatabaseExceptionHandler>()
		// 	.AddExceptionHandler<HttpGlobalExceptionHandler>();

		// services.AddControllers()
		// 	.AddOData(options =>
		// 	{
		// 		options.Filter()
		// 			.OrderBy()
		// 			.Count()
		// 			.SetMaxTop(1000);
		// 	})
		// 	.AddNewtonsoftJson(options =>
		// 	{
		// 		options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
		// 	});

		services.AddValidatorsFromAssemblyContaining<IPresentationAssemblyMarker>(includeInternalTypes: true)
			// .AddValidatorsFromAssemblyContaining<IApplicationContractsAssemblyMarker>(includeInternalTypes: true)
			// .AddFluentValidationAutoValidation(config => config.DisableDataAnnotationsValidation = true)
			.Configure<ApiBehaviorOptions>(options =>
			{
				options.InvalidModelStateResponseFactory = context => throw new ValidationException(context
					.ModelState
					.Where(a => a.Value?.ValidationState == ModelValidationState.Invalid)
					.Select(a => new ValidationFailure(a.Key,
						string.Join(", ", a.Value!.Errors.Select(b => b.ErrorMessage)), a.Value.AttemptedValue)));
			});

		// services.AddHttpContextAccessor();
		// services.AddScoped<IRequestContext, RequestContext>();

		return services;
	}

	public static IApplicationBuilder UseWebApi(this IApplicationBuilder app)
	{
		app.UseExceptionHandler();

		app.UseRouting();

		app.UseEndpoints(endpoints =>
		{
			// Adds Liveness
			endpoints.MapGet(
					"/",
					Results<Ok<VersionResponse>, ProblemHttpResult> () =>
					{
						var version = Assembly.GetExecutingAssembly()
							.GetName()
							.Version?.ToString();

						if (version is null)
						{
							return TypedResults.Problem("Version unknown", statusCode: StatusCodes.Status422UnprocessableEntity);
						}

						return TypedResults.Ok(new VersionResponse(version));
					})
				.Produces<VersionResponse>(StatusCodes.Status200OK)
				.Produces<VersionResponse>(StatusCodes.Status422UnprocessableEntity);
		});
		// app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
		// app.UseStatusCodePages(async statusCodeContext =>
		// {
		// 	statusCodeContext.HttpContext.Response.ContentType = MediaTypeNames.Text.Plain;
		//
		// 	await statusCodeContext.HttpContext.Response.WriteAsync(
		// 		$"Status Code Page: {statusCodeContext.HttpContext.Response.StatusCode}");
		// });

		return app;
	}
}
