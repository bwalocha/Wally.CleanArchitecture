using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Wally.CleanArchitecture.MicroService.WebApi.Handlers;

internal sealed class HttpGlobalExceptionHandler : IExceptionHandler
{
	private readonly ILogger<HttpGlobalExceptionHandler> _logger;
	private readonly IProblemDetailsService _problemDetailsService;
	// private readonly IRequestContext _requestContext;

	public HttpGlobalExceptionHandler(IProblemDetailsService problemDetailsService, /*IRequestContext requestContext,*/ ILogger<HttpGlobalExceptionHandler> logger)
	{
		_problemDetailsService = problemDetailsService;
		// _requestContext = requestContext;
		_logger = logger;
	}

	public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
	{
		if (exception is TaskCanceledException)
		{
			return true;
		}

		_logger.LogError(new EventId(exception.HResult), exception, exception.Message);

		switch (exception)
		{
			default:
				Debugger.Break();
				httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
				return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
				{
					HttpContext = httpContext,
					Exception = exception,
					ProblemDetails = new ProblemDetails
					{
						Type = exception.GetType().Name,
						Title = "Internal Server Error",
						Status = StatusCodes.Status500InternalServerError,
						Instance = httpContext.Request.Path,
						// Detail = $"Internal Server Error. CorrelationId: '{_requestContext.CorrelationId}'.",
						Detail = "Internal Server Error",
						// Extensions = []
					},
					// AdditionalMetadata = 
				});
		}
	}
}
