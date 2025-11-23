using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using Wally.CleanArchitecture.ApiGateway.Infrastructure.DI.Microsoft.Extensions;
using Wally.CleanArchitecture.ApiGateway.Infrastructure.DI.Microsoft.Models;

namespace Wally.CleanArchitecture.ApiGateway.WebApi;

public class Startup
{
	public Startup(IConfiguration configuration)
	{
		Configuration = configuration;
	}

	public IConfiguration Configuration { get; }

	// This method gets called by the runtime. Use this method to add services to the container.
	public void ConfigureServices(IServiceCollection services)
	{
		services.AddInfrastructure(Configuration);
	}

#pragma warning disable S2325
	public void Configure(
		IApplicationBuilder app,
		IWebHostEnvironment env,
		IHostApplicationLifetime appLifetime,
		ILogger<Startup> logger,
		IOptions<AppSettings> options,
		IFeatureManager featureManager)
	{
		appLifetime.ApplicationStarted.Register(() =>
			logger.LogInformation("The 'Wally.CleanArchitecture.ApiGateway.WebApi' is started"));
		appLifetime.ApplicationStopping.Register(() =>
			logger.LogInformation("The 'Wally.CleanArchitecture.ApiGateway.WebApi' is stopping"));
		appLifetime.ApplicationStopped.Register(() =>
			logger.LogInformation("The 'Wally.CleanArchitecture.ApiGateway.WebApi' is stopped"));

		app.UseInfrastructure(env, options, featureManager);
	}
#pragma warning restore S2325
}
