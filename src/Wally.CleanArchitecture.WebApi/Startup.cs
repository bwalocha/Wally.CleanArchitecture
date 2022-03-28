using System.Reflection;

using FluentValidation.AspNetCore;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using Wally.CleanArchitecture.Application.Users.Commands;
using Wally.CleanArchitecture.Contracts.Requests.User;
using Wally.CleanArchitecture.Infrastructure.DI.Microsoft.Extensions;
using Wally.CleanArchitecture.Infrastructure.DI.Microsoft.Models;
using Wally.CleanArchitecture.Persistence;
using Wally.CleanArchitecture.WebApi.Filters;
using Wally.CleanArchitecture.WebApi.Hubs;

namespace Wally.CleanArchitecture.WebApi;

public class Startup
{
	public Startup(IConfiguration configuration, IWebHostEnvironment env)
	{
		Configuration = configuration;
		Environment = env;
	}

	public IConfiguration Configuration { get; }

	public IWebHostEnvironment Environment { get; }

	public AppSettings AppSettings { get; } = new();

	// This method gets called by the runtime. Use this method to add services to the container.
	public void ConfigureServices(IServiceCollection services)
	{
		Configuration.Bind(AppSettings);

		services.AddControllers(settings => { settings.Filters.Add(typeof(HttpGlobalExceptionFilter)); })
			.AddFluentValidation(
				config =>
				{
					config.ImplicitlyValidateChildProperties = true;
					config.RegisterValidatorsFromAssemblyContaining<UpdateUserRequestValidator>();
					config.RegisterValidatorsFromAssemblyContaining<UpdateUserCommandValidator>();
				})
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

		services.AddCqrs();
		services.AddApiCors(AppSettings.Cors);
		services.AddSwagger(Assembly.GetExecutingAssembly());
		services.AddHealthChecks(Configuration);
		services.AddDbContext(Configuration);
		services.AddMapper();
		services.AddMessaging(Configuration);
		services.AddEventHub();
	}

	public void Configure(
		IApplicationBuilder app,
		IWebHostEnvironment env,
		IHostApplicationLifetime appLifetime,
		ILogger<Startup> logger,
		ApplicationDbContext dbContext)
	{
		appLifetime.ApplicationStarted.Register(
			() => logger.LogInformation("The 'Wally.CleanArchitecture' is started"));
		appLifetime.ApplicationStopping.Register(
			() => logger.LogInformation("The 'Wally.CleanArchitecture' is stopping"));
		appLifetime.ApplicationStopped.Register(
			() => logger.LogInformation("The 'Wally.CleanArchitecture' is stopped"));

		// Configure the HTTP request pipeline.
		if (env.IsDevelopment())
		{
			app.UseDeveloperExceptionPage();
			app.UseSwagger(AppSettings.SwaggerAuthentication);
		}

		// App is hosted by Docker, HTTPS is not required inside container
		// app.UseHttpsRedirection();

		app.UseRouting();
		app.UseApiCors();

		app.UseAuthentication();
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
