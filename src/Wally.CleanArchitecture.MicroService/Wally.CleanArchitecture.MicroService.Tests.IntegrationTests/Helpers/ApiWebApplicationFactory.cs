using System.Collections.Generic;
using System.IO;
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
using Wally.CleanArchitecture.MicroService.Application.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Tests.IntegrationTests.Helpers;

public class ApiWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>, IAsyncLifetime
	where TStartup : class
{
	private const string MsSqlContainerName = "Wally.CleanArchitecture.MicroService.Tests";
	private const string MsSqlContainerImageName = "mcr.microsoft.com/mssql/server:2022-CU13-ubuntu-22.04";
	// private const long MsSqlContainerMemoryLimit = 2L * 1024 * 1024 * 1024; // 2GB

	private readonly MsSqlContainer _dbContainer = new MsSqlBuilder()
		.WithImage(MsSqlContainerImageName)
		.WithName(MsSqlContainerName)
		// .WithCreateParameterModifier(a => a.HostConfig.Memory = MsSqlContainerMemoryLimit)
		.WithCreateParameterModifier(a =>
		{
			a.HostConfig.Memory = 0;
			a.HostConfig.MemorySwap = -1;
			a.HostConfig.PidsLimit = 0;
		})
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

	public async Task InitializeAsync()
	{
		await Task.WhenAll(
			_dbContainer.StartAsync()
			/*, _kafkaContainer.StartAsync()*/
		);
	}

	public new async Task DisposeAsync()
	{
		await Task.WhenAll(
			_dbContainer.StopAsync()
			/*, _kafkaContainer.StopAsync()*/
		);

		await _dbContainer.DisposeAsync();
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
		return base.CreateHostBuilder() !.ConfigureAppConfiguration(configurationBuilder =>
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
		builder.ConfigureTestServices(services =>
		{
			services.AddTransient<IBus, BusStub>();

			services.RemoveAll<TimeProvider>();
			services.AddSingleton<TimeProvider, FakeTimeProvider>();
		});
	}
}
