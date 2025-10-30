using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Wally.CleanArchitecture.MicroService.Domain.Exceptions;
// using Wally.CleanArchitecture.MicroService.Application.Abstractions;

namespace Wally.CleanArchitecture.MicroService.WebApi.Handlers;

internal sealed class AuthorizationExceptionHandler : IExceptionHandler
{
	private readonly ILogger<AuthorizationExceptionHandler> _logger;
	private readonly IProblemDetailsService _problemDetailsService;
	// private readonly IRequestContext _requestContext;

	public AuthorizationExceptionHandler(IProblemDetailsService problemDetailsService, /*IRequestContext requestContext,*/ ILogger<AuthorizationExceptionHandler> logger)
	{
		_problemDetailsService = problemDetailsService;
		// _requestContext = requestContext;
		_logger = logger;
	}

	public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
	{
		_logger.LogError(new EventId(exception.HResult), exception, exception.Message);

		switch (exception)
		{
			case PermissionDeniedException _:
				httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
				return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
				{
					HttpContext = httpContext,
					Exception = exception,
					ProblemDetails = new ProblemDetails
					{
						Type = exception.GetType()
							.Name,
						Title = "Authorization Exception",
						Status = StatusCodes.Status403Forbidden,
						Instance = httpContext.Request.Path,
						Detail = exception.Message,
						// Extensions = []
					},
					// AdditionalMetadata = 
				});
			default:
				return false;
		}
	}
}
