using System.Threading;
using System.Threading.Tasks;
using Wally.CleanArchitecture.Application.Abstractions;
using Wally.CleanArchitecture.Contracts.Responses.Users;

namespace Wally.CleanArchitecture.Application.Users.Queries;

public class GetUserQueryHandler : QueryHandler<GetUserQuery, GetUserResponse>
{
	private readonly IUserRepository _usersRepository;

	public GetUserQueryHandler(IUserRepository usersRepository)
	{
		_usersRepository = usersRepository;
	}

	public override Task<GetUserResponse> HandleAsync(GetUserQuery query, CancellationToken cancellationToken)
	{
		return _usersRepository.GetAsync<GetUserResponse>(query.Id, cancellationToken);
	}
}
