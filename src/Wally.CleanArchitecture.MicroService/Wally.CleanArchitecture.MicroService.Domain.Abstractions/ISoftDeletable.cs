namespace Wally.CleanArchitecture.MicroService.Domain.Abstractions;

public interface ISoftDeletable
{
	bool IsDeleted { get; }

	DateTimeOffset? DeletedAt { get; }

	UserId? DeletedById { get; }
}
