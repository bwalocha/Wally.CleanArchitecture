using System;
using System.IO;
using System.Linq;

using HealthChecks.UI.Core.Data;

using MassTransit;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Wally.CleanArchitecture.MicroService.Infrastructure.Persistence;
using Wally.Lib.DDD.Abstractions.DomainNotifications;

using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Wally.CleanArchitecture.MicroService.Tests.IntegrationTests.Helpers;

public class ApiWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
	protected override IHostBuilder CreateHostBuilder()
	{
		return base.CreateHostBuilder()!.ConfigureAppConfiguration(
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

				var notifications = services.Where(
					a => a.ServiceType.IsGenericType && a.ServiceType.GetGenericTypeDefinition() ==
						typeof(IDomainNotificationHandler<>));

				foreach (var descriptor in notifications.ToArray())
				{
					services.Remove(descriptor);
				}

				// Add ApplicationDbContext using an in-memory database for testing.
				Action<DbContextOptionsBuilder> options = optionsAction =>
				{
					optionsAction.UseInMemoryDatabase("InMemoryDbForTesting");
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

	public TService GetRequiredService<TService>() where TService : notnull
	{
		var scopeFactory = Services.GetService<IServiceScopeFactory>();
		return scopeFactory!.CreateScope()
			.ServiceProvider.GetRequiredService<TService>();
	}
}
