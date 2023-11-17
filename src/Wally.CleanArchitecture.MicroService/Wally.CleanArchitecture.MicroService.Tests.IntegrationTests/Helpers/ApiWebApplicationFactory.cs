using System;
using System.IO;

using MassTransit;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

using Wally.CleanArchitecture.MicroService.Infrastructure.Persistence;

namespace Wally.CleanArchitecture.MicroService.Tests.IntegrationTests.Helpers;

public class ApiWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
	public TService GetRequiredService<TService>() where TService : notnull
	{
		var scopeFactory = Services.GetService<IServiceScopeFactory>();
		return scopeFactory!.CreateScope()
			.ServiceProvider.GetRequiredService<TService>();
	}

	protected override IHostBuilder CreateHostBuilder()
	{
		return base.CreateHostBuilder() !.ConfigureAppConfiguration(
				configurationBuilder =>
				{
					configurationBuilder.SetBasePath(Directory.GetCurrentDirectory())
						.AddJsonFile("appsettings.json", false)
						.AddJsonFile("appsettings.IntegrationTests.json", false);
				})
			.UseEnvironment("IntegrationTests");
	}

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		builder.ConfigureTestServices(
			services =>
			{
				services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
				services.RemoveAll<ApplicationDbContext>();

				// Add ApplicationDbContext using an in-memory database for testing.
				var databaseName = $"InMemoryDbForTesting_{Guid.NewGuid()}";
				Action<DbContextOptionsBuilder> options = optionsAction =>
				{
					optionsAction.UseInMemoryDatabase(databaseName);
					optionsAction.ConfigureWarnings(a => { a.Ignore(InMemoryEventId.TransactionIgnoredWarning); });
					optionsAction.EnableSensitiveDataLogging();
				};

				services.AddDbContext<DbContext, ApplicationDbContext>(options);

				// Build the service provider.
				var sp = services.BuildServiceProvider();

				// Create a scope to obtain a reference to the database
				// context (ApplicationDbContext).
				using var scope = sp.CreateScope();
				var scopedServices = scope.ServiceProvider;

				// Ensure the database is created.
				var database = scopedServices.GetRequiredService<DbContext>();
				database.Database.EnsureCreated();

				services.AddTransient<IBus, BusStub>();
			});
	}
}
