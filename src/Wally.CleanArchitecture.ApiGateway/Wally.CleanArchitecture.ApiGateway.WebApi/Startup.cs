using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Wally.CleanArchitecture.ApiGateway.Infrastructure.DI.Microsoft;

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

		app.UseInfrastructure(env);
	}
}
