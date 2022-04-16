using System;
using System.Diagnostics.CodeAnalysis;

using Wally.Lib.DDD.Abstractions.DomainEvents;

namespace Wally.CleanArchitecture.MicroService.Domain.Users;

[ExcludeFromCodeCoverage]
public class UserCreatedDomainEvent : DomainEvent
{
	public UserCreatedDomainEvent(Guid id, string name)
	{
		Id = id;
		Name = name;
	}

	public Guid Id { get; }

	public string Name { get; }
}
