namespace Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.Abstractions.Models;

public class DbContextSettings
{
	public DatabaseProviderType ProviderType { get; init; }

	public bool IsMigrationEnabled { get; init; } = false;
}
