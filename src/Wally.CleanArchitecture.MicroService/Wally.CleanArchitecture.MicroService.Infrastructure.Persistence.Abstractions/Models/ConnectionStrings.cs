namespace Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.Abstractions.Models;

public class ConnectionStrings
{
	public string Database { get; init; } = null!;

	public string ServiceBus { get; init; } = null!;
}
