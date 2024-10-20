﻿using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Wally.CleanArchitecture.MicroService.Application.Abstractions;
using Wally.CleanArchitecture.MicroService.Domain.Users;

namespace Wally.CleanArchitecture.MicroService.Application.Users.Commands;

public class DeleteUserCommandAuthorizationHandler : ICommandAuthorizationHandler<DeleteUserCommand, Unit>
{
	private readonly IRequestContext _requestContext;

	public DeleteUserCommandAuthorizationHandler(IRequestContext requestContext)
	{
		_requestContext = requestContext;
	}

	public Task<AuthorizationHandlerResult> HandleAsync(DeleteUserCommand command, CancellationToken cancellationToken)
		=> Task.FromResult(_requestContext.UserId.Equals(command.UserId)
			? AuthorizationHandlerResult.Unauthorized
			: AuthorizationHandlerResult.Succeeded);
}
