using AutoMapper;
using Wally.CleanArchitecture.MicroService.Application.Users;
using Wally.CleanArchitecture.MicroService.Domain.Users;
using Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.Persistence;

public class UserRepository : Repository<User, UserId>, IUserRepository
{
	public UserRepository(ApplicationDbContext context, IMapper mapper)
		: base(context, mapper)
	{
	}
}
