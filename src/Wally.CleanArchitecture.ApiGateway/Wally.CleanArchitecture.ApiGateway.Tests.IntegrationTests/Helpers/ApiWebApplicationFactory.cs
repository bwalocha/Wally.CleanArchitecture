using System.IO;
using System.Linq;
using HealthChecks.UI.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Wally.CleanArchitecture.ApiGateway.Tests.IntegrationTests.Helpers;

public class ApiWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
	protected override IHostBuilder CreateHostBuilder()
	{
		return base.CreateHostBuilder() !.ConfigureAppConfiguration(
			configurationBuilder =>
			{
				configurationBuilder.SetBasePath(Directory.GetCurrentDirectory())
					.AddJsonFile("appsettings.IntegrationTests.json", false);
			});
	}

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		builder.ConfigureServices(
			services =>
			{
				// Remove the app's ApplicationDbContext registration.
				var descriptors = services.Where(
						a => a.ServiceType.IsSubclassOf(typeof(DbContextOptions)) ||
							a.ServiceType.IsSubclassOf(typeof(DbContext)))
					.Where(
						a => a.ServiceType != typeof(DbContextOptions<HealthChecksDb>) &&
							a.ServiceType != typeof(HealthChecksDb));

				foreach (var descriptor in descriptors.ToArray())
				{
					services.Remove(descriptor);
				}
			});
	}

	public TService GetRequiredService<TService>() where TService : notnull
	{
		var scopeFactory = Services.GetService<IServiceScopeFactory>();
		return scopeFactory!.CreateScope()
			.ServiceProvider.GetRequiredService<TService>();
	}
}
