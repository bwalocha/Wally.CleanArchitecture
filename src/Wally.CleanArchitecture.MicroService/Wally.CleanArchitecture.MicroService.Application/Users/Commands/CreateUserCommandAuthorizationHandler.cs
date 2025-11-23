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

	public ValueTask<AuthorizationHandlerResult> HandleAsync(CreateUserCommand command,
		CancellationToken cancellationToken)
	{
		return ValueTask.FromResult(AuthorizationHandlerResult.Succeeded);
	}
}
