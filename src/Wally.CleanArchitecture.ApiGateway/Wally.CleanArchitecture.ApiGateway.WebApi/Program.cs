using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

// using Azure.Identity;

namespace Wally.CleanArchitecture.ApiGateway.WebApi;

[ExcludeFromCodeCoverage]
public static class Program
{
	private const string _azureADManagedIdentityClientIdConfigName = "AzureADManagedIdentityClientId";
	private const string _keyVaultNameConfigName = "KeyVaultName";
	private const bool _reloadOnChange = false;

	public static int Main(string[] args)
	{
		var configurationBuilder = new ConfigurationBuilder();
		var configuration = ConfigureDefaultConfiguration(configurationBuilder)
			.Build();

		Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration)
			.CreateLogger();

		try
		{
			Log.Information("Starting host...");
			CreateHostBuilder(args)
				.Build()
				.Run();
		}
		catch (Exception ex)
		{
			Log.Fatal(ex, "Host terminated unexpectedly");

			return 1;
		}
		finally
		{
			Log.CloseAndFlush();
		}

		return 0;
	}

	private static IConfigurationBuilder ConfigureDefaultConfiguration(IConfigurationBuilder configurationBuilder)
	{
		var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

		configurationBuilder.Sources.Clear();

		return configurationBuilder.SetBasePath(Directory.GetCurrentDirectory())
			.AddJsonFile("appsettings.json", false, _reloadOnChange)
			.AddJsonFile($"appsettings.{env}.json", true, _reloadOnChange)
			.AddJsonFile("serilog.json", false, _reloadOnChange)
			.AddJsonFile($"serilog.{env}.json", true, _reloadOnChange)
			.AddEnvironmentVariables();
	}

	/// <summary>
	///     https://learn.microsoft.com/en-us/azure/active-directory/managed-identities-azure-resources/tutorial-windows-vm-access-nonaad#grant-access
	/// </summary>
	/// <param name="configurationBuilder">The ConfigurationBuilder.</param>
	/// <returns></returns>
	private static IConfigurationBuilder ConfigureAppConfiguration(IConfigurationBuilder configurationBuilder)
	{
		/*var configuration = configurationBuilder.Build();
		var keyVaultName = configuration[_keyVaultNameConfigName];
		var keyVaultUrl = new Uri($"https://{keyVaultName}.vault.azure.net/");
		var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions
		{
			ManagedIdentityClientId = configuration[_azureADManagedIdentityClientIdConfigName]
		});

		return configurationBuilder.AddAzureKeyVault(keyVaultUrl, credential);*/
		return configurationBuilder;
	}

	private static IHostBuilder CreateHostBuilder(string[] args)
	{
		return Host.CreateDefaultBuilder(args)
			.ConfigureAppConfiguration(a => ConfigureAppConfiguration(ConfigureDefaultConfiguration(a)))
			.UseSerilog()
			.UseDefaultServiceProvider(opt => { opt.ValidateScopes = true; })
			.ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
	}
}
