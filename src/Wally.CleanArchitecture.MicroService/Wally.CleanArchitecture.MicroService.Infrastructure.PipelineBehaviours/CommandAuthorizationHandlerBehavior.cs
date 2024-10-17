using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Wally.CleanArchitecture.MicroService.Application.Abstractions;
using Wally.CleanArchitecture.MicroService.Domain.Exceptions;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.PipelineBehaviours;

public class CommandAuthorizationHandlerBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : ICommand<TResponse>
{
	private readonly ICommandAuthorizationHandler<TRequest, TResponse>? _authorizationHandler;

	public CommandAuthorizationHandlerBehavior(
		ICommandAuthorizationHandler<TRequest, TResponse>? authorizationHandler = null)
	{
		_authorizationHandler = authorizationHandler;
	}

	public async Task<TResponse> Handle(
		TRequest request,
		RequestHandlerDelegate<TResponse> next,
		CancellationToken cancellationToken)
	{
		var authorizationResult = AuthorizationHandlerResult.Unauthorized;

		if (_authorizationHandler != null)
		{
			authorizationResult = await _authorizationHandler.HandleAsync(request, cancellationToken);
		}

		if (authorizationResult != AuthorizationHandlerResult.Succeeded)
		{
			throw new PermissionDeniedException();
		}

		return await next();
	}
}
