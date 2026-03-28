using System.Diagnostics.CodeAnalysis;

namespace Wally.CleanArchitecture.MicroService.Domain.Users;

[ExcludeFromCodeCoverage]
public class UserCreatedDomainEvent : DomainEvent
{
	internal UserCreatedDomainEvent(UserId id)
	{
		Id = id;
	}

	public UserId Id { get; }
}
