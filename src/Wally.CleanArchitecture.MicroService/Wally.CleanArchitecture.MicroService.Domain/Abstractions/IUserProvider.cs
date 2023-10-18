using Wally.CleanArchitecture.MicroService.Domain.Users;

namespace Wally.CleanArchitecture.MicroService.Domain.Abstractions;

public interface IUserProvider
{
	UserId GetCurrentUserId();
}
