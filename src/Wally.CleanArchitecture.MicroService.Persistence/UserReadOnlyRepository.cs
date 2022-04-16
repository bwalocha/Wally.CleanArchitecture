using AutoMapper;

using Microsoft.EntityFrameworkCore;

using Wally.CleanArchitecture.MicroService.Application.Users;
using Wally.CleanArchitecture.MicroService.Domain.Users;
using Wally.CleanArchitecture.MicroService.Persistence.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Persistence;

public class UserReadOnlyRepository : ReadOnlyRepository<User>, IUserReadOnlyRepository
{
	public UserReadOnlyRepository(DbContext context, IMapper mapper)
		: base(context, mapper)
	{
	}
}
