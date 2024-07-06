using System;
using System.IO;
using System.Threading.Tasks;
using DotNet.Testcontainers.Containers;
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
using Microsoft.Extensions.Options;
using Testcontainers.MariaDb;
using Testcontainers.MsSql;
using Testcontainers.MySql;
using Testcontainers.PostgreSql;
using Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Models;
using Wally.CleanArchitecture.MicroService.Infrastructure.Persistence;
using Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.SqlServer;
using Xunit;

namespace Wally.CleanArchitecture.MicroService.Tests.IntegrationTests.Helpers;

public class ApiWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>, IAsyncLifetime
	where TStartup : class
{
	private DockerContainer _dbContainer = null!;

	public Task InitializeAsync()
	{
		/*await _dbContainer
			.StartAsync();*/
		return Task.CompletedTask;
	}

	public new async Task DisposeAsync()
	{
		await _dbContainer
			.StopAsync();
	}

	public TService GetRequiredService<TService>()
		where TService : notnull
	{
		var scopeFactory = Services.GetService<IServiceScopeFactory>();
		return scopeFactory!.CreateScope()
			.ServiceProvider.GetRequiredService<TService>();
	}

	public async Task<int> SeedAsync(params object[] entities)
	{
		var dbContext = GetRequiredService<DbContext>();

		await dbContext.AddRangeAsync(entities);
		return await dbContext.SaveChangesAsync();
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
				ConfigureDbContext(services);
				services.AddTransient<IBus, BusStub>();
			});
	}

	private AppSettings GetAppSettings(IServiceCollection services)
	{
		// Create a scope to obtain a reference to the AppConfiguration
		using var scope = services.BuildServiceProvider()
			.CreateScope();
		var scopedServices = scope.ServiceProvider;

		return scopedServices.GetRequiredService<IOptions<AppSettings>>()
			.Value;
	}

	private void ConfigureDbContext(IServiceCollection services)
	{
		services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
		services.RemoveAll<ApplicationDbContext>();

		var settings = GetAppSettings(services);

		Action<DbContextOptionsBuilder> options;

		switch (settings.Database.ProviderType)
		{
			case DatabaseProviderType.InMemory:
				// Add ApplicationDbContext using an in-memory database for testing.
				var databaseName = $"InMemoryDbForTesting_{Guid.NewGuid()}";
				options = optionsAction =>
				{
					optionsAction.UseInMemoryDatabase(databaseName);
					optionsAction.ConfigureWarnings(a => { a.Ignore(InMemoryEventId.TransactionIgnoredWarning); });
					optionsAction.EnableSensitiveDataLogging();
				};
				break;
			case DatabaseProviderType.MariaDb:
				_dbContainer = new MariaDbBuilder()
					.WithUsername(Guid.NewGuid()
						.ToString())
					.WithPassword(Guid.NewGuid()
						.ToString())
					.WithDatabase(Guid.NewGuid()
						.ToString())
					.Build();
				_dbContainer
					.StartAsync()
					.ConfigureAwait(false)
					.GetAwaiter()
					.GetResult();
				options = optionsAction =>
				{
					optionsAction.UseMySql(
						((IDatabaseContainer)_dbContainer).GetConnectionString(),
						MySqlServerVersion.LatestSupportedServerVersion,
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
				break;
			case DatabaseProviderType.MySql:
				_dbContainer = new MySqlBuilder()
					.WithUsername(Guid.NewGuid()
						.ToString())
					.WithPassword(Guid.NewGuid()
						.ToString())
					.WithDatabase(Guid.NewGuid()
						.ToString())
					.Build();
				_dbContainer
					.StartAsync()
					.ConfigureAwait(false)
					.GetAwaiter()
					.GetResult();
				options = optionsAction =>
				{
					optionsAction.UseMySql(
						((IDatabaseContainer)_dbContainer).GetConnectionString(),
						MySqlServerVersion.LatestSupportedServerVersion,
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
				break;
			case DatabaseProviderType.PostgreSQL:
				// Add ApplicationDbContext using a PostgreSQL database for testing.
				_dbContainer = new PostgreSqlBuilder()
					.WithUsername(Guid.NewGuid()
						.ToString())
					.WithPassword(Guid.NewGuid()
						.ToString())
					.WithDatabase(Guid.NewGuid()
						.ToString())
					.Build();
				_dbContainer
					.StartAsync()
					.ConfigureAwait(false)
					.GetAwaiter()
					.GetResult();
				options = optionsAction =>
				{
					optionsAction.UseNpgsql(
						((IDatabaseContainer)_dbContainer).GetConnectionString(),
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
				break;
			case DatabaseProviderType.SqlServer:
				// Add ApplicationDbContext using a SqlServer database for testing.
				_dbContainer = new MsSqlBuilder()
					.WithImage("mcr.microsoft.com/mssql/server:2022-latest")
					.WithPassword(Guid.NewGuid()
						.ToString())
					.WithCleanUp(true)
					.Build();
				_dbContainer
					.StartAsync()
					.ConfigureAwait(false)
					.GetAwaiter()
					.GetResult();
				options = optionsAction =>
				{
					optionsAction.UseSqlServer(
						((IDatabaseContainer)_dbContainer).GetConnectionString(),
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
				break;
			default:
				throw new NotSupportedException();
		}

		services.AddDbContext<DbContext, ApplicationDbContext>(options);

		// Create a scope to obtain a reference to the AppConfiguration
		using var scope = services.BuildServiceProvider()
			.CreateScope();

		// Ensure the database is created.
		var database = scope.ServiceProvider.GetRequiredService<DbContext>();
		database.Database.EnsureCreated();
	}

	protected override void Dispose(bool disposing)
	{
		_dbContainer?.DisposeAsync()
			.AsTask()
			.ConfigureAwait(false)
			.GetAwaiter()
			.GetResult();

		base.Dispose(disposing);
	}
}
