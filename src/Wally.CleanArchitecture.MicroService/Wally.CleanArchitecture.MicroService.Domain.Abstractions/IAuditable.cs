namespace Wally.CleanArchitecture.MicroService.Domain.Abstractions;

public interface IAuditable
{
	DateTimeOffset CreatedAt { get; }

	UserId CreatedById { get; }

	DateTimeOffset? ModifiedAt { get; }

	UserId? ModifiedById { get; }
}
