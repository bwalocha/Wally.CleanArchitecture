using AutoMapper;

using Microsoft.EntityFrameworkCore;

using Wally.CleanArchitecture.Application.Users;
using Wally.CleanArchitecture.Domain.Users;
using Wally.CleanArchitecture.Persistence.Abstractions;

namespace Wally.CleanArchitecture.Persistence;

public class UserReadOnlyRepository : ReadOnlyRepository<User>, IUserReadOnlyRepository
{
	public UserReadOnlyRepository(DbContext context, IMapper mapper)
		: base(context, mapper)
	{
	}
}
