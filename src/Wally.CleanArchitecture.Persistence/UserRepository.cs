using AutoMapper;
using Wally.CleanArchitecture.Application.Users;
using Wally.CleanArchitecture.Domain.Users;

namespace Wally.CleanArchitecture.Persistence;

public class UserRepository : Repository<User>, IUserRepository
{
	public UserRepository(ApplicationDbContext context, IMapper mapper)
		: base(context, mapper)
	{
	}
}
