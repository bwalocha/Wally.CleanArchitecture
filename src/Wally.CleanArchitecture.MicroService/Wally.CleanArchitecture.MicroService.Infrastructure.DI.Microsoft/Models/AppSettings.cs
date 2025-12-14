namespace Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Models;

// TODO: Remove Setters,
// extract interfaces
// and add ConventionTests
public class AppSettings
{
	public AuthenticationSettings Authentication { get; } = new(); // TODO: only with no Api Gateway?

	public AuthenticationSettings SwaggerAuthentication { get; } = new();
	
	public OpenTelemetrySettings OpenTelemetry { get; } = new();

	public DbContextSettings Database { get; } = new();

	public MessageBrokerType MessageBroker { get; init; }

	public ConnectionStrings ConnectionStrings { get; } = new();

	public MapperSettings MapperSettings { get; } = new();
	
	public SchedulerSettings SchedulerSettings { get; } = new();
}
