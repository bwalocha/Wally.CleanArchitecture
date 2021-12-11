using Wally.CleanArchitecture.Domain.Abstractions;
using Wally.CleanArchitecture.Domain.Users;

namespace Wally.CleanArchitecture.Application.Users
{
	public interface IUserRepository : IRepository<User>
	{
	}
}
