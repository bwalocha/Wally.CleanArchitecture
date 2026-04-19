using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;
using Wally.CleanArchitecture.ApiGateway.Infrastructure.DI.Microsoft.Models;

namespace Wally.CleanArchitecture.ApiGateway.Infrastructure.DI.Microsoft.Extensions;

public static class OpenApiExtensions
{
	private const string ContactName = "Wally";
	private const string ContactEmail = "b.walocha@gmail.com";
	private const string ContactUrl = "https://wally.ovh";

	private const string LicenseName = "MIT";
	private const string LicenseUrl = "https://opensource.org/licenses/MIT";

	private const string OpenApiInfoVersion = "v1";

	private const string AppName = "Wally.CleanArchitecture";
	
	public static IServiceCollection AddOpenApi(this IServiceCollection services, Assembly assembly,
		AppSettings settings)
	{
		// services.AddOpenApi(); // TODO: https://www.youtube.com/watch?v=8yI4gD1HruY&ab_channel=NickChapsas

		// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
		services.AddEndpointsApiExplorer();
		// services.AddODataApiExplorer();
		services.AddSwaggerGen(options =>
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
			xmlFilename = $"{assembly.GetName().Name}.Contracts.xml";
			options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

			// options.OperationFilter<ResponseTypesOperationFilter>();
			// options.OperationFilter<ODataQueryOptionsOperationFilter>();

			// options.DocumentFilter<SchemasFilter>();
		});
		
		return services;
	}

	public static IApplicationBuilder UseOpenApi(this IApplicationBuilder app, AuthenticationSettings settings,
		ReverseProxySettings reverseProxySettings)
	{
		// app.MapOpenApi(); // TODO: https://www.youtube.com/watch?v=8yI4gD1HruY&ab_channel=NickChapsas

		app.UseSwagger(setupAction: null)
			.UseSwaggerUI(options =>
			{
				options.OAuthClientId(settings.ClientId);
				options.OAuthClientSecret(settings.ClientSecret);
				options.OAuthUsePkce();
				options.DefaultModelsExpandDepth(0);
				options.RoutePrefix = "swagger";

				foreach (var route in reverseProxySettings.Routes)
				{
					foreach (var prefix in route.Transforms!.SelectMany(a => a.Values)
								.Distinct())
					{
						var url = $"https://localhost:5001{prefix}/swagger/v1/swagger.json";
						var name = $"Wally.CleanArchitecture API [{route.ClusterId}]";

						options.SwaggerEndpoint(url, name);
					}
				}

				options.ConfigObject.Urls = options.ConfigObject.Urls?.DistinctBy(a => a.Url);
			});

		return app;
	}
}
