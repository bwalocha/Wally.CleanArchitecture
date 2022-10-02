using FluentValidation;
using FluentValidation.AspNetCore;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using Wally.CleanArchitecture.MicroService.Application.Users.Commands;
using Wally.CleanArchitecture.MicroService.Contracts.Requests.Users;
using Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft;
using Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Extensions;
using Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Models;
using Wally.CleanArchitecture.MicroService.WebApi.Filters;
using Wally.CleanArchitecture.MicroService.WebApi.Hubs;

namespace Wally.CleanArchitecture.MicroService.WebApi;

public class Startup
{
	public Startup(IConfiguration configuration)
	{
		Configuration = configuration;
	}

	/// <summary>
	///     Gets Configuration data
	/// </summary>
	public IConfiguration Configuration { get; }

	/// <summary>
	///     Gets Application Settings data
	/// </summary>
	public AppSettings AppSettings { get; } = new();

	// This method gets called by the runtime. Use this method to add services to the container.
	public void ConfigureServices(IServiceCollection services)
	{
		Configuration.Bind(AppSettings);

		services.AddControllers(settings => { settings.Filters.Add(typeof(HttpGlobalExceptionFilter)); })
			.AddOData(
				options =>
				{
					options.Filter()
						.OrderBy()
						.Count()
						.SetMaxTop(1000);
				})
			.AddNewtonsoftJson(
				options => { options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore; });

		// TODO: move to Infrastructure
		services.AddValidatorsFromAssemblyContaining<UpdateUserRequestValidator>();
		services.AddValidatorsFromAssemblyContaining<UpdateUserCommandValidator>();
		services.AddFluentValidationAutoValidation(config => config.DisableDataAnnotationsValidation = true);
		services.AddFluentValidationClientsideAdapters(/*config => config.ClientValidatorFactories*/);

		services.AddInfrastructure(AppSettings);
	}

	public void Configure(
		IApplicationBuilder app,
		IWebHostEnvironment env,
		IHostApplicationLifetime appLifetime,
		ILogger<Startup> logger,
		DbContext dbContext)
	{
		appLifetime.ApplicationStarted.Register(
			() => logger.LogInformation("The 'Wally.CleanArchitecture.MicroService' is started"));
		appLifetime.ApplicationStopping.Register(
			() => logger.LogInformation("The 'Wally.CleanArchitecture.MicroService' is stopping"));
		appLifetime.ApplicationStopped.Register(
			() => logger.LogInformation("The 'Wally.CleanArchitecture.MicroService' is stopped"));

		// Configure the HTTP request pipeline.
		if (env.IsDevelopment())
		{
			app.UseDeveloperExceptionPage();
			app.UseSwagger(AppSettings.SwaggerAuthentication);
		}

		// If the App is hosted by Docker, HTTPS is not required inside container
		// app.UseHttpsRedirection();

		app.UseRouting();

		// app.UseAuthentication(); // TODO: Consider only for ApiGateway
		app.UseAuthorization();
		app.UseHealthChecks();
		app.UseEndpoints(
			endpoints =>
			{
				endpoints.MapControllers();
				endpoints.MapGet(
					"/",
					async context =>
					{
						await context.Response.WriteAsync(
							$"v{GetType().Assembly.GetName().Version}",
							context.RequestAborted);
					});
			});

		app.UseDbContext(dbContext, AppSettings.Database);
		app.UseMessaging();
		app.UseEventHub<EventHub>();
	}
}
