namespace Wally.CleanArchitecture.ApiGateway.Infrastructure.DI.Microsoft.Models;

public class AuthenticationSettings
{
	public string Authority { get; init; } = null!;

	public string ClientId { get; init; } = null!;

	public string ClientSecret { get; init; } = null!;
}
