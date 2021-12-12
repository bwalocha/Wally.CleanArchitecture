using Serilog;

namespace Wally.CleanArchitecture.WebApi
{
	public static class Program
	{
		private const bool _reloadOnChange = false;

		private static IConfiguration Configuration { get; } = new ConfigurationBuilder()
			.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", false, _reloadOnChange)
			.AddJsonFile(
				$"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json",
				true,
				_reloadOnChange).AddJsonFile("serilog.json", true, _reloadOnChange).AddJsonFile(
				$"serilog.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json",
				true,
				_reloadOnChange).AddEnvironmentVariables().Build();

		public static int Main(string[] args)
		{
			Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(Configuration).CreateLogger();

			try
			{
				Log.Information("Starting host...");
				CreateHostBuilder(args).Build().Run();
			}
			catch (Exception ex)
			{
				Log.Fatal(ex, "Host terminated unexpectedly.");

				return 1;
			}
			finally
			{
				Log.CloseAndFlush();
			}

			return 0;
		}

		private static IHostBuilder CreateHostBuilder(string[] args)
		{
			return Host.CreateDefaultBuilder(args).UseSerilog()
				.UseDefaultServiceProvider(opt => { opt.ValidateScopes = true; })
				.ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
		}
	}
}
