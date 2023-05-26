using System;
using System.IO;
using System.Reflection;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

using Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Models;
using Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Swagger;

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

				options.OperationFilter<ResponseTypesOperationFilter>();
				options.OperationFilter<ODataQueryOptionsOperationFilter>();
				
				options.DocumentFilter<SchemasFilter>();
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
				opt.DefaultModelsExpandDepth(0);
			});

		return app;
	}
}
