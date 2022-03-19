namespace Wally.CleanArchitecture.Infrastructure.DI.Microsoft.Models;

public class AuthenticationSettings
{
	public string Authority { get; set; } = string.Empty;

	public string ClientId { get; set; } = string.Empty;

	public string ClientSecret { get; set; } = string.Empty;
}
