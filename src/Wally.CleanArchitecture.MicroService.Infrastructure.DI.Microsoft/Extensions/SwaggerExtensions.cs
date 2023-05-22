using System;
using System.IO;
using System.Linq;
using System.Reflection;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

using Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Models;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Extensions;

public static class SwaggerExtensions
{
	public static IServiceCollection AddSwagger(this IServiceCollection services, Assembly assembly)
	{
		// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
		services.AddEndpointsApiExplorer();
		services.AddSwaggerGen(
			options =>
			{
				options.SwaggerDoc(
					"v1",
					new OpenApiInfo
					{
						Version = "v1",
						Title = "Wally.CleanArchitecture API",
						Description = "An ASP.NET Core Web API for managing 'Wally.CleanArchitecture' items",

						// TermsOfService = new Uri("https://example.com/terms"),
						Contact = new OpenApiContact
						{
							Name = "Wally", Email = "b.walocha@gmail.com", Url = new Uri("https://wally.best"),
						},
						License = new OpenApiLicense
						{
							Name = "MIT", Url = new Uri("https://opensource.org/licenses/MIT"),
						},
					});

				var xmlFilename = $"{assembly.GetName().Name}.xml";
				options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

				options.OperationFilter<ODataQueryOptionsOperationFilter>();
			});

		return services;
	}

	public static IApplicationBuilder UseSwagger(this IApplicationBuilder app, AuthenticationSettings settings)
	{
		app.UseSwagger();
		app.UseSwaggerUI(
			opt =>
			{
				// opt.SwaggerEndpoint("v1/swagger.json", "Wally.CleanArchitecture WebApi v1");
				opt.OAuthClientId(settings.ClientId);
				opt.OAuthClientSecret(settings.ClientSecret);
				opt.OAuthUsePkce();
			});

		return app;
	}

	private class ODataQueryOptionsOperationFilter : IOperationFilter
	{
		public void Apply(OpenApiOperation operation, OperationFilterContext context)
		{
			var odataQueryParameterTypes = context.ApiDescription.ActionDescriptor.Parameters
				.Where(p => p.ParameterType.IsAssignableTo(typeof(ODataQueryOptions)))
				.ToList();

			if (!odataQueryParameterTypes.Any())
			{
				return;
			}

			// Remove the large queryOptions field from Swagger which gets added by default...
			foreach (var queryParamType in odataQueryParameterTypes)
			{
				var paramToRemove = operation.Parameters.SingleOrDefault(p => p.Name == queryParamType.Name);
				if (paramToRemove != null)
				{
					operation.Parameters.Remove(paramToRemove);
				}
			}

			// ...and add our own query parameters.
			operation.Parameters.Add(
				new OpenApiParameter
				{
					Name = "$filter",
					In = ParameterLocation.Query,
					Description = "Filter the results",
					Required = false,
					Schema = new OpenApiSchema { Type = "string", },
				});

			operation.Parameters.Add(
				new OpenApiParameter
				{
					Name = "$orderby",
					In = ParameterLocation.Query,
					Description = "Order the results",
					Required = false,
					Schema = new OpenApiSchema { Type = "string", },
				});

			operation.Parameters.Add(
				new OpenApiParameter
				{
					Name = "$select",
					In = ParameterLocation.Query,
					Description = "Select the properties to be returned in the response",
					Required = false,
					Schema = new OpenApiSchema { Type = "string", },
				});

			operation.Parameters.Add(
				new OpenApiParameter
				{
					Name = "$top",
					In = ParameterLocation.Query,
					Description = "Limit the number of results returned",
					Required = false,
					Schema = new OpenApiSchema { Type = "integer", Format = "int32", },
				});

			operation.Parameters.Add(
				new OpenApiParameter
				{
					Name = "$skip",
					In = ParameterLocation.Query,
					Description = "Skip the specified number of results",
					Required = false,
					Schema = new OpenApiSchema { Type = "integer", Format = "int32", },
				});
		}
	}
}