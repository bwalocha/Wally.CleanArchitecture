using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft;

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

	// This method gets called by the runtime. Use this method to add services to the container.
	public void ConfigureServices(IServiceCollection services)
	{
		services.AddInfrastructure(Configuration);
	}

	public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime appLifetime,
		ILogger<Startup> logger)
	{
		appLifetime.ApplicationStarted.Register(() =>
			logger.LogInformation("The 'Wally.CleanArchitecture.MicroService' is started"));
		appLifetime.ApplicationStopping.Register(() =>
			logger.LogInformation("The 'Wally.CleanArchitecture.MicroService' is stopping"));
		appLifetime.ApplicationStopped.Register(() =>
			logger.LogInformation("The 'Wally.CleanArchitecture.MicroService' is stopped"));

		app.UseInfrastructure(env);
	}
}
