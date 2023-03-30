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

				options.OperationFilter<SwaggerDefaultValues>();
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

	private class SwaggerDefaultValues : IOperationFilter
	{
		/// <summary>
		///     Applies the filter to the specified operation using the given context.
		/// </summary>
		/// <param name="operation">The operation to apply the filter to.</param>
		/// <param name="context">The current operation filter context.</param>
		public void Apply(OpenApiOperation operation, OperationFilterContext context)
		{
			var apiDescription = context.ApiDescription;

			// operation.Deprecated |= apiDescription.IsDeprecated();

			// REF: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/1752#issue-663991077
			foreach (var responseType in context.ApiDescription.SupportedResponseTypes)
			{
				// REF: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/blob/b7cf75e7905050305b115dd96640ddd6e74c7ac9/src/Swashbuckle.AspNetCore.SwaggerGen/SwaggerGenerator/SwaggerGenerator.cs#L383-L387
				var responseKey = responseType.IsDefaultResponse ? "default" : responseType.StatusCode.ToString();
				var response = operation.Responses[responseKey];

				foreach (var contentType in response.Content.Keys)
				{
					if (!responseType.ApiResponseFormats.Any(x => x.MediaType == contentType))
					{
						response.Content.Remove(contentType);
					}
				}
			}

			if (operation.Parameters == null)
			{
				return;
			}

			// REF: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/412
			// REF: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/pull/413
			foreach (var parameter in operation.Parameters.ToArray())
			{
				var description = apiDescription.ParameterDescriptions.First(p => p.Name == parameter.Name);
				if (description.Type.BaseType == typeof(ODataQueryOptions))
				{
					operation.Parameters.Remove(parameter);
					continue;
				}

				parameter.Description ??= description.ModelMetadata?.Description;
				parameter.Required |= description.IsRequired;
			}
		}
	}
}
