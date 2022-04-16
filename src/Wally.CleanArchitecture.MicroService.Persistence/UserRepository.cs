using AutoMapper;

using Microsoft.EntityFrameworkCore;

using Wally.CleanArchitecture.MicroService.Application.Users;
using Wally.CleanArchitecture.MicroService.Domain.Users;
using Wally.CleanArchitecture.MicroService.Persistence.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Persistence;

public class UserRepository : Repository<User>, IUserRepository
{
	public UserRepository(DbContext context, IMapper mapper)
		: base(context, mapper)
	{
	}
}
