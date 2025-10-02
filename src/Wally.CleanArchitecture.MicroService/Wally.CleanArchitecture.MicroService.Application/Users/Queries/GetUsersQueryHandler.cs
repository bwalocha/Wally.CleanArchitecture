using Wally.CleanArchitecture.MicroService.Application.Abstractions;
using Wally.CleanArchitecture.MicroService.Application.Users.Requests;
using Wally.CleanArchitecture.MicroService.Application.Users.Results;

namespace Wally.CleanArchitecture.MicroService.Application.Users.Queries;

public class GetUsersQueryHandler : QueryHandler<GetUsersQuery, PagedResult<GetUsersResult>>
{
	private readonly IUserReadOnlyRepository _userRepository;

	public GetUsersQueryHandler(IUserReadOnlyRepository userRepository)
	{
		_userRepository = userRepository;
	}

	public override async ValueTask<PagedResult<GetUsersResult>> HandleAsync(
		GetUsersQuery query,
		CancellationToken cancellationToken)
	{
		return await _userRepository.GetAsync<GetUsersRequest, GetUsersResult>(query.QueryOptions, cancellationToken);
	}
}
