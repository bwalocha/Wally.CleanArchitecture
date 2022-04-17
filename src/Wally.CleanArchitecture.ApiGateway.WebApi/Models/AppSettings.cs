namespace Wally.CleanArchitecture.ApiGateway.WebApi.Models;

// TODO: Remove Setters,
// extract interfaces
// and add ConventionTests
public class AppSettings
{
	public AuthenticationSettings Authentication { get; } = new();

	public CorsSettings Cors { get; } = new();
}
