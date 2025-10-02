using Wally.CleanArchitecture.MicroService.Application.Abstractions;
using Wally.CleanArchitecture.MicroService.Application.Users.Results;

namespace Wally.CleanArchitecture.MicroService.Application.Users.Queries;

public class GetUserQueryHandler : QueryHandler<GetUserQuery, GetUserResult>
{
	private readonly IUserReadOnlyRepository _userRepository;

	public GetUserQueryHandler(IUserReadOnlyRepository userRepository)
	{
		_userRepository = userRepository;
	}

	public override async ValueTask<GetUserResult> HandleAsync(GetUserQuery query, CancellationToken cancellationToken)
	{
		return await _userRepository.GetAsync<GetUserResult>(query.UserId, cancellationToken);
	}
}
