using HealthChecks.UI.Client;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Wally.CleanArchitecture.ApiGateway.WebApi.Extensions;
using Wally.CleanArchitecture.ApiGateway.WebApi.Models;

namespace Wally.CleanArchitecture.ApiGateway.WebApi;

public class Startup
{
	private const string ReverseProxy = nameof(ReverseProxy);

	public Startup(IConfiguration configuration)
	{
		Configuration = configuration;
	}

	public IConfiguration Configuration { get; }

	/// <summary>
	///     Gets Application Settings data
	/// </summary>
	public AppSettings AppSettings { get; } = new();

	// This method gets called by the runtime. Use this method to add services to the container.
	public void ConfigureServices(IServiceCollection services)
	{
		services.AddReverseProxy()
			.LoadFromConfig(Configuration.GetSection(ReverseProxy));

		// Add HealthChecks (configuration in AppSettings.json file)
		services.AddHealthChecks()
			.AddVersionHealthCheck();
		services.AddHealthChecksUI()
			.AddInMemoryStorage();

		services.AddApiCors(AppSettings.Cors);
	}

	public void Configure(
		IApplicationBuilder app,
		IWebHostEnvironment env,
		IHostApplicationLifetime appLifetime,
		ILogger<Startup> logger)
	{
		appLifetime.ApplicationStarted.Register(
			() => logger.LogInformation("The 'Wally.CleanArchitecture.ApiGateway.WebApi' is started"));
		appLifetime.ApplicationStopping.Register(
			() => logger.LogInformation("The 'Wally.CleanArchitecture.ApiGateway.WebApi' is stopping"));
		appLifetime.ApplicationStopped.Register(
			() => logger.LogInformation("The 'Wally.CleanArchitecture.ApiGateway.WebApi' is stopped"));

		// Configure the HTTP request pipeline.
		if (env.IsDevelopment())
		{
			app.UseDeveloperExceptionPage();
		}

		// App is hosted by Docker, HTTPS is not required inside container
		// app.UseHttpsRedirection();

		app.UseRouting();
		app.UseApiCors();

		// app.UseAuthentication(); // TODO: configure Auth2
		app.UseHealthChecks(
				"/healthChecks",
				new HealthCheckOptions
				{
					Predicate = _ => true, ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
				})
			.UseHealthChecksUI();
		app.UseEndpoints(
			endpoints =>
			{
				endpoints.MapGet(
					"/",
					async context =>
					{
						await context.Response.WriteAsync(
							$"v{GetType().Assembly.GetName().Version}",
							context.RequestAborted);
					});
				endpoints.MapReverseProxy();
			});
	}
}
