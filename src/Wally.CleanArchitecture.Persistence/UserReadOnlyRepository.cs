using AutoMapper;

using Wally.CleanArchitecture.Application.Users;
using Wally.CleanArchitecture.Domain.Users;
using Wally.CleanArchitecture.Persistence.Abstractions;

namespace Wally.CleanArchitecture.Persistence;

public class UserReadOnlyRepository : ReadOnlyRepository<User>, IUserReadOnlyRepository
{
	public UserReadOnlyRepository(ApplicationDbContext context, IMapper mapper)
		: base(context, mapper)
	{
	}
}
