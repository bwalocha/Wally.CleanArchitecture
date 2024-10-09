using System.Diagnostics.CodeAnalysis;
using Wally.CleanArchitecture.MicroService.Domain.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Domain.Users;

[ExcludeFromCodeCoverage]
public class UserCreatedDomainEvent : DomainEvent
{
	public UserCreatedDomainEvent(UserId id)
	{
		Id = id;
	}

	public UserId Id { get; }
}
