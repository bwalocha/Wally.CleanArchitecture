using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Models;
using Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Swagger;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Extensions;

public static class OpenApiExtensions
{
	private const string ContactName = "Wally";
	private const string ContactEmail = "b.walocha@gmail.com";
	private const string ContactUrl = "https://wally.best";
	
	private const string LicenseName = "MIT";
	private const string LicenseUrl = "https://opensource.org/licenses/MIT";

	private const string OpenApiInfoVersion = "v1";
	
	private const string AppName = "Wally.CleanArchitecture";

	public static IServiceCollection AddOpenApi(this IServiceCollection services, Assembly assembly)
	{
		// services.AddOpenApi(); // TODO: https://www.youtube.com/watch?v=8yI4gD1HruY&ab_channel=NickChapsas

		// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
		services.AddEndpointsApiExplorer();
		// services.AddODataApiExplorer();
		services.AddSwaggerGen(
			options =>
			{
				options.SwaggerDoc(
					OpenApiInfoVersion,
					new OpenApiInfo
					{
						Version = OpenApiInfoVersion,
						Title = $"{AppName} API",
						Description = $"An ASP.NET Core Web API for managing '{AppName}' items",

						// TermsOfService = new Uri("https://example.com/terms"),
						Contact = new OpenApiContact
						{
							Name = ContactName,
							Email = ContactEmail,
							Url = new Uri(ContactUrl),
						},
						License = new OpenApiLicense
						{
							Name = LicenseName,
							Url = new Uri(LicenseUrl),
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

	public static IApplicationBuilder UseOpenApi(this IApplicationBuilder app)
	{
		// app.MapOpenApi(); // TODO: https://www.youtube.com/watch?v=8yI4gD1HruY&ab_channel=NickChapsas

		app.UseSwagger(setupAction: null)
			.UseSwaggerUI(
				options =>
				{
					var settings = app.ApplicationServices.GetRequiredService<IOptions<AppSettings>>();

					// options.SwaggerEndpoint("v1/swagger.json", "Wally.CleanArchitecture WebApi v1");
					options.OAuthClientId(settings.Value.SwaggerAuthentication.ClientId);
					options.OAuthClientSecret(settings.Value.SwaggerAuthentication.ClientSecret);

					// TODO:
					// options.OAuthScopes(string.Join(", ", settings.Value.SwaggerAuthentication.Scopes.Values));
					options.OAuthUsePkce();

					options.OAuthAppName(AppName);
					options.EnablePersistAuthorization();
					options.DefaultModelsExpandDepth(0);
				});

		return app;
	}
}
