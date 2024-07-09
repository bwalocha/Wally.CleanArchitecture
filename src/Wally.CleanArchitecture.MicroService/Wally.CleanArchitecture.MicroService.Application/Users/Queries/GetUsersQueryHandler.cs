using System.Threading;
using System.Threading.Tasks;
using Wally.CleanArchitecture.MicroService.Application.Abstractions;
using Wally.CleanArchitecture.MicroService.Application.Contracts;
using Wally.CleanArchitecture.MicroService.Application.Contracts.Users.Requests;
using Wally.CleanArchitecture.MicroService.Application.Contracts.Users.Responses;

namespace Wally.CleanArchitecture.MicroService.Application.Users.Queries;

public class GetUsersQueryHandler : QueryHandler<GetUsersQuery, PagedResponse<GetUsersResponse>>
{
	private readonly IUserReadOnlyRepository _userRepository;

	public GetUsersQueryHandler(IUserReadOnlyRepository userRepository)
	{
		_userRepository = userRepository;
	}

	public override Task<PagedResponse<GetUsersResponse>> HandleAsync(
		GetUsersQuery query,
		CancellationToken cancellationToken)
	{
		return _userRepository.GetAsync<GetUsersRequest, GetUsersResponse>(query.QueryOptions, cancellationToken);
	}
}
