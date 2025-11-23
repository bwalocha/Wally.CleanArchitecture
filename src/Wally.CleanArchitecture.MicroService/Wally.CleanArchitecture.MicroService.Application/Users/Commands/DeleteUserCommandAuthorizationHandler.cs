using Mediator;
using Wally.CleanArchitecture.MicroService.Application.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Application.Users.Commands;

public class DeleteUserCommandAuthorizationHandler : ICommandAuthorizationHandler<DeleteUserCommand, Unit>
{
	private readonly IRequestContext _requestContext;

	public DeleteUserCommandAuthorizationHandler(IRequestContext requestContext)
	{
		_requestContext = requestContext;
	}

	public ValueTask<AuthorizationHandlerResult> HandleAsync(DeleteUserCommand command,
		CancellationToken cancellationToken)
	{
		return ValueTask.FromResult(_requestContext.UserId.Equals(command.UserId)
			? AuthorizationHandlerResult.Unauthorized
			: AuthorizationHandlerResult.Succeeded);
	}
}
