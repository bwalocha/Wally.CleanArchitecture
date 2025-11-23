using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.Abstractions;

namespace Wally.CleanArchitecture.MicroService.WebApi.Handlers;

internal sealed class NotFoundExceptionHandler : IExceptionHandler
{
	private readonly ILogger<NotFoundExceptionHandler> _logger;

	private readonly IProblemDetailsService _problemDetailsService;
	// private readonly IRequestContext _requestContext;

	public NotFoundExceptionHandler(IProblemDetailsService problemDetailsService, /*IRequestContext requestContext,*/
		ILogger<NotFoundExceptionHandler> logger)
	{
		_problemDetailsService = problemDetailsService;
		// _requestContext = requestContext;
		_logger = logger;
	}

	public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
		CancellationToken cancellationToken)
	{
		if (exception is TaskCanceledException)
		{
			return true;
		}

		_logger.LogError(new EventId(exception.HResult), exception, exception.Message);

		switch (exception)
		{
			case INotFound _:
				httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
				return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
				{
					HttpContext = httpContext,
					Exception = exception,
					ProblemDetails = new ProblemDetails
					{
						Type = exception.GetType()
							.Name,
						Title = "Resource Not Found Exception",
						Status = StatusCodes.Status404NotFound,
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
