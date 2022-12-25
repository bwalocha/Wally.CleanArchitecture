namespace Wally.CleanArchitecture.ApiGateway.Infrastructure.DI.Microsoft.Models;

public class AppSettings
{
	public AuthenticationSettings Authentication { get; } = new();

	public CorsSettings Cors { get; } = new();
}
