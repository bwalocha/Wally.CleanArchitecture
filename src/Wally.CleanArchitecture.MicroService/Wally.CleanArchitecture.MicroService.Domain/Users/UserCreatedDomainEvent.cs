using System.Diagnostics.CodeAnalysis;
using Wally.CleanArchitecture.MicroService.Domain.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Domain.Users;

[ExcludeFromCodeCoverage]
public class UserCreatedDomainEvent : DomainEvent
{
	public UserCreatedDomainEvent(UserId id, string name)
	{
		Id = id;
		Name = name;
	}

	public UserId Id { get; }

	public string Name { get; }
}
