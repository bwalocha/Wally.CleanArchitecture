namespace Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Models;

// TODO: Remove Setters,
// extract interfaces
// and add ConventionTests
public class AppSettings
{
	public AuthenticationSettings Authentication { get; } = new();

	public AuthenticationSettings SwaggerAuthentication { get; } = new();

	public CorsSettings Cors { get; } = new();

	public DbContextSettings Database { get; } = new();
}
