using System.Linq;
using Wally.CleanArchitecture.MicroService.Application.Abstractions;
using Wally.CleanArchitecture.MicroService.Application.Users;
using Wally.CleanArchitecture.MicroService.Domain.Abstractions;
using Wally.CleanArchitecture.MicroService.Domain.Users;
using Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.Persistence;

public class UserReadOnlyRepository : ReadOnlyRepository<User, UserId>, IUserReadOnlyRepository
{
	public UserReadOnlyRepository(IUnitOfWork unitOfWork, IMapper mapper)
		: base(unitOfWork, mapper)
	{
	}

	protected override IQueryable<User> ApplySearch(IQueryable<User> query, string term)
	{
		return query.Where(a => a.Name.StartsWith(term));
	}
}
