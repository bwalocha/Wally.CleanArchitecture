using System;
using System.Collections.Generic;
using System.Data.Common;
using EntityFramework.Exceptions.Common;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

// using Wally.CleanArchitecture.MicroService.Application.Abstractions;

namespace Wally.CleanArchitecture.MicroService.WebApi.Handlers;

internal sealed class DatabaseExceptionHandler : IExceptionHandler
{
	private readonly ILogger<DatabaseExceptionHandler> _logger;

	private readonly IProblemDetailsService _problemDetailsService;
	// private readonly IRequestContext _requestContext;

	public DatabaseExceptionHandler(IProblemDetailsService problemDetailsService, /*IRequestContext requestContext,*/
		ILogger<DatabaseExceptionHandler> logger)
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
			case UniqueConstraintException _:
			case CannotInsertNullException _:
			case MaxLengthExceededException _:
			case NumericOverflowException _:
			case ReferenceConstraintException _:
			case DbUpdateException _:
				httpContext.Response.StatusCode = StatusCodes.Status409Conflict;
				return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
				{
					HttpContext = httpContext,
					Exception = exception,
					ProblemDetails = new ProblemDetails
					{
						Type = exception.GetType()
							.Name,
						Title = "Database Exception",
						Status = StatusCodes.Status409Conflict,
						Instance = httpContext.Request.Path,
						Detail = "Please refer to the errors property for additional details.",
						Extensions = exception.InnerException! is DbException dbException
							? new Dictionary<string, object?>
							{
								["errorCode"] = dbException.ErrorCode,
								["message"] = dbException.Message,
							}
							: new Dictionary<string, object?>(),
					},
					// AdditionalMetadata =
				});
			default:
				return false;
		}
	}
}
