using System.Diagnostics.CodeAnalysis;

namespace Wally.CleanArchitecture.MicroService.Domain.Abstractions;

[ExcludeFromCodeCoverage]
public class DomainEvent
{
	protected DomainEvent()
	{
	}

	// TODO: Consider to add a CreatedAt property
	// public DateTimeOffset CreatedAt { get; private set; } = new(DateTime.UtcNow);
}
