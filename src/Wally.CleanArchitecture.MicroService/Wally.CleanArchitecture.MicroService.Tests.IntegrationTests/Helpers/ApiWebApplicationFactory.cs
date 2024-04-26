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
using Testcontainers.MsSql;
using Wally.CleanArchitecture.MicroService.Infrastructure.Persistence;
using Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.SqlServer;

namespace Wally.CleanArchitecture.MicroService.Tests.IntegrationTests.Helpers;

public class ApiWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
	where TStartup : class
{
	private readonly MsSqlContainer _dbContainer = new MsSqlBuilder()
		.WithImage("mcr.microsoft.com/mssql/server:2022-latest")
		.WithPassword(Guid.NewGuid()
			.ToString())
		.WithCleanUp(true)
		.Build();

	protected override IHost CreateHost(IHostBuilder builder)
	{
		_dbContainer
			.StartAsync()
			.ConfigureAwait(false)
			.GetAwaiter()
			.GetResult();

		return base.CreateHost(builder);
	}

	protected override void Dispose(bool disposing)
	{
		// _dbContainer.StopAsync();
		_dbContainer.DisposeAsync()
			.AsTask()
			.ConfigureAwait(false)
			.GetAwaiter()
			.GetResult();

		base.Dispose(disposing);
	}

	public TService GetRequiredService<TService>()
		where TService : notnull
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
				/*var databaseName = $"InMemoryDbForTesting_{Guid.NewGuid()}";
				Action<DbContextOptionsBuilder> options = optionsAction =>
				{
					optionsAction.UseInMemoryDatabase(databaseName);
					optionsAction.ConfigureWarnings(a => { a.Ignore(InMemoryEventId.TransactionIgnoredWarning); });
					optionsAction.EnableSensitiveDataLogging();
				};*/

				// Add ApplicationDbContext using a SqlServer database for testing.
				Action<DbContextOptionsBuilder> options = optionsAction =>
				{
					optionsAction.UseSqlServer(
						_dbContainer.GetConnectionString(),
						opt =>
						{
							opt.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
							opt.MigrationsAssembly(
								typeof(IInfrastructureSqlServerAssemblyMarker).Assembly.GetName()
									.Name);
						});
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
