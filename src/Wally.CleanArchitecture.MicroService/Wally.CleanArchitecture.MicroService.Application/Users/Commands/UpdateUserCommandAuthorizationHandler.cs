using Mediator;
using Wally.CleanArchitecture.MicroService.Application.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Application.Users.Commands;

public class UpdateUserCommandAuthorizationHandler : ICommandAuthorizationHandler<UpdateUserCommand, Unit>
{
	private readonly IRequestContext _requestContext;

	public UpdateUserCommandAuthorizationHandler(IRequestContext requestContext)
	{
		_requestContext = requestContext;
	}

	public ValueTask<AuthorizationHandlerResult> HandleAsync(UpdateUserCommand command, CancellationToken cancellationToken)
	{
		return ValueTask.FromResult(AuthorizationHandlerResult.Succeeded);
	}
}
