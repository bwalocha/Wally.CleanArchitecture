namespace Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Models;

public class AuthenticationSettings
{
	public string Authority { get; set; } = null!;

	public string ClientId { get; set; } = null!;

	public string ClientSecret { get; set; } = null!;
}
