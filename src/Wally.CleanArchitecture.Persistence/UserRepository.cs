using AutoMapper;

using Microsoft.EntityFrameworkCore;

using Wally.CleanArchitecture.Application.Users;
using Wally.CleanArchitecture.Domain.Users;
using Wally.CleanArchitecture.Persistence.Abstractions;

namespace Wally.CleanArchitecture.Persistence;

public class UserRepository : Repository<User>, IUserRepository
{
	public UserRepository(DbContext context, IMapper mapper)
		: base(context, mapper)
	{
	}
}
