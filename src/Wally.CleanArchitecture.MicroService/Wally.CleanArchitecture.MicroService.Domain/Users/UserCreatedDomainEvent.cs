namespace Wally.CleanArchitecture.MicroService.Domain.Users;

[ExcludeFromCodeCoverage]
public class UserCreatedDomainEvent : DomainEvent
{
	public UserId Id { get; }

	internal UserCreatedDomainEvent(UserId id)
	{
		Id = id;
	}
}
