using System.Threading;
using System.Threading.Tasks;

using Wally.CleanArchitecture.Application.Abstractions;
using Wally.CleanArchitecture.Contracts.Requests.User;
using Wally.CleanArchitecture.Contracts.Responses.Users;
using Wally.Lib.DDD.Abstractions.Responses;

namespace Wally.CleanArchitecture.Application.Users.Queries;

public class GetUsersQueryHandler : QueryHandler<GetUsersQuery, PagedResponse<GetUsersResponse>>
{
	private readonly IUserRepository _usersRepository;

	public GetUsersQueryHandler(IUserRepository usersRepository)
	{
		_usersRepository = usersRepository;
	}

	public override Task<PagedResponse<GetUsersResponse>> HandleAsync(
		GetUsersQuery query,
		CancellationToken cancellationToken)
	{
		return _usersRepository.GetAsync<GetUsersRequest, GetUsersResponse>(query.QueryOptions, cancellationToken);
	}
}
