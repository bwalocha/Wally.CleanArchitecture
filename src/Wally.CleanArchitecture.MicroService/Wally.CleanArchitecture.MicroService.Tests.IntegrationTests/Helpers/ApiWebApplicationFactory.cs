using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Time.Testing;
using Testcontainers.MsSql;

namespace Wally.CleanArchitecture.MicroService.Tests.IntegrationTests.Helpers;

public class ApiWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>, IAsyncLifetime
	where TStartup : class
{
	private readonly MsSqlContainer _dbContainer = new MsSqlBuilder()
		// .WithImage("mcr.microsoft.com/mssql/server:2022-CU13-ubuntu-22.04") // rollback from server:2022-latest due to issue with health check
		.WithImage("mcr.microsoft.com/mssql/server:2022-CU13-ubuntu-22.04")
		// .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
		.WithName("Wally.CleanArchitecture.MicroService.Tests")
		.WithCleanUp(true)
		.WithReuse(true)
		.Build();

	/*
	 private readonly KafkaContainer _kafkaContainer = new KafkaBuilder()
		.WithImage("confluentinc/cp-kafka:6.2.10")
		.WithReuse(true)
		.Build();
	*/
	
	/*
	 private static readonly AzuriteContainer AzuriteContainer = new AzuriteBuilder()
		.WithImage("mcr.microsoft.com/azure-storage/azurite:3.34.0")
		// .WithName($"azurite-integration-tests")
		// .WithPortBinding(10000, 10000)
		// .WithPortBinding(10001, 10001)
		// .WithPortBinding(10002, 10002)
		.WithCleanUp(true)
		.Build();
	*/

	public Task InitializeAsync()
	{
		return Task.WhenAll(_dbContainer.StartAsync() /*, _kafkaContainer.StartAsync()*/);
	}

	public new Task DisposeAsync()
	{
		return Task.WhenAll(
			_dbContainer.StopAsync()
			// _dbContainer.DisposeAsync().AsTask()
			/*, _kafkaContainer.StopAsync()*/);
	}

	public TService GetRequiredService<TService>()
		where TService : notnull
	{
		var scopeFactory = Services.GetRequiredService<IServiceScopeFactory>();

		return scopeFactory
			.CreateScope()
			.ServiceProvider
			.GetRequiredService<TService>();
	}

	public async Task<int> SeedAsync(params object[] entities)
	{
		var dbContext = GetRequiredService<DbContext>();
		await dbContext.AddRangeAsync(entities);

		return await dbContext.SaveChangesAsync();
	}

	public ApiWebApplicationFactory<TStartup> RemoveAll<TEntity>()
		where TEntity : class
	{
		var dbContext = GetRequiredService<DbContext>();
		dbContext.RemoveRange(dbContext.Set<TEntity>()
			.IgnoreQueryFilters());
		dbContext.SaveChanges();

		return this;
	}

	public Task<int> RemoveAllAsync<TEntity>()
		where TEntity : class
	{
		var dbContext = GetRequiredService<DbContext>();
		dbContext.RemoveRange(dbContext.Set<TEntity>()
			.IgnoreQueryFilters());

		return dbContext.SaveChangesAsync();
	}

	protected override IHostBuilder CreateHostBuilder()
	{
		return base.CreateHostBuilder() !.ConfigureAppConfiguration(
				configurationBuilder =>
				{
					configurationBuilder.SetBasePath(Directory.GetCurrentDirectory())
						.AddJsonFile("appsettings.json", false)
						.AddJsonFile("appsettings.IntegrationTests.json", false)
						.AddInMemoryCollection([
							new KeyValuePair<string, string?>("ConnectionStrings:Database",
								_dbContainer.GetConnectionString()),
							// new KeyValuePair<string, string>("Database:ServiceBus", _kafkaContainer.GetBootstrapAddress())
						]);
				})
			.UseEnvironment("IntegrationTests");
	}

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		builder.ConfigureTestServices(
			services =>
			{
				services.AddTransient<IBus, BusStub>();

				services.RemoveAll<TimeProvider>();
				services.AddSingleton<TimeProvider, FakeTimeProvider>();
			});
	}
}
