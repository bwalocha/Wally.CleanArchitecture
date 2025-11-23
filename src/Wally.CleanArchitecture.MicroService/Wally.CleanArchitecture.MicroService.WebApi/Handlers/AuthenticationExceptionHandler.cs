using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Wally.CleanArchitecture.MicroService.WebApi.Handlers;

internal sealed class AuthenticationExceptionHandler : IExceptionHandler
{
	private readonly ILogger<AuthenticationExceptionHandler> _logger;

	private readonly IProblemDetailsService _problemDetailsService;
	// private readonly IRequestContext _requestContext;

	public AuthenticationExceptionHandler(
		IProblemDetailsService problemDetailsService, /*IRequestContext requestContext,*/
		ILogger<AuthenticationExceptionHandler> logger)
	{
		_problemDetailsService = problemDetailsService;
		// _requestContext = requestContext;
		_logger = logger;
	}

	public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
		CancellationToken cancellationToken)
	{
		_logger.LogError(new EventId(exception.HResult), exception, exception.Message);

		switch (exception)
		{
			case UnauthorizedAccessException _:
				httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
				return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
				{
					HttpContext = httpContext,
					Exception = exception,
					ProblemDetails = new ProblemDetails
					{
						Type = exception.GetType()
							.Name,
						Title = "Authentication Exception",
						Status = StatusCodes.Status401Unauthorized,
						Instance = httpContext.Request.Path,
						Detail = exception.Message,
						// Extensions = []
					},
					// AdditionalMetadata = 
				});
				break;
			default:
				return false;
		}
	}
}
