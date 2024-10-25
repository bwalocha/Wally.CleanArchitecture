using System.Threading;
using System.Threading.Tasks;
using Wally.CleanArchitecture.MicroService.Application.Abstractions;
using Wally.CleanArchitecture.MicroService.Domain.Users;

namespace Wally.CleanArchitecture.MicroService.Application.Users.Commands;

public class CreateUserCommandAuthorizationHandler : ICommandAuthorizationHandler<CreateUserCommand, UserId>
{
	private readonly IRequestContext _requestContext;

	public CreateUserCommandAuthorizationHandler(IRequestContext requestContext)
	{
		_requestContext = requestContext;
	}

	public Task<AuthorizationHandlerResult> HandleAsync(CreateUserCommand command, CancellationToken cancellationToken)
	{
		return Task.FromResult(AuthorizationHandlerResult.Succeeded);
	}
}
