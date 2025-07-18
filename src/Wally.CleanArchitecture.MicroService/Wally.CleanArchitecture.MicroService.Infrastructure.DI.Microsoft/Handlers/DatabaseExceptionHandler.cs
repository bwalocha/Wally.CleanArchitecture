using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using EntityFramework.Exceptions.Common;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
// using Wally.CleanArchitecture.MicroService.Application.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Handlers;

internal sealed class DatabaseExceptionHandler : IExceptionHandler
{
	private readonly ILogger<DatabaseExceptionHandler> _logger;
	private readonly IProblemDetailsService _problemDetailsService;
	// private readonly IRequestContext _requestContext;

	public DatabaseExceptionHandler(IProblemDetailsService problemDetailsService, /*IRequestContext requestContext,*/ ILogger<DatabaseExceptionHandler> logger)
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
			// case PermissionDeniedException _:
			// 	HandlePermissionDeniedException(context);
			// 	break;
			// case DomainException _:
			// case FluentValidation.ValidationException _:
			// 	HandleDomainValidationException(context);
			// 	break;
			// case UnauthorizedAccessException _:
			// 	HandleUnauthorizedAccessException(context);
			// 	break;
			// case INotFound _:
			// 	HandleResourceNotFoundException(context);
			// 	break;
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
				// HandleUndefinedExceptions(context, _requestContext);
				// Debugger.Break();
				// break;
				return false;
		}
	}

	// private static void HandleSqlException(ExceptionContext context)
	// {
	// 	var problemDetails = new ValidationProblemDetails
	// 	{
	// 		// Title = "Validation Error",
	// 		Instance = context.HttpContext.Request.Path,
	// 		Status = StatusCodes.Status409Conflict,
	// 		Detail = "Please refer to the errors property for additional details.",
	// 	};
	//
	// 	if (context.Exception.InnerException! is DbException exception)
	// 	{
	// 		problemDetails.Errors.Add(
	// 			"Database",
	// 			[
	// 				exception.ErrorCode.ToString(), exception.Message,
	// 			]);
	// 	}
	//
	// 	context.Result = new ConflictObjectResult(problemDetails);
	// }

	// private static void HandlePermissionDeniedException(ExceptionContext context)
	// {
	// 	var problemDetails = new ProblemDetails
	// 	{
	// 		Title = "Permission Denied",
	// 		Instance = context.HttpContext.Request.Path,
	// 		Status = StatusCodes.Status403Forbidden,
	// 		Detail = context.Exception.Message,
	// 	};
	//
	// 	context.Result = new ObjectResult(problemDetails)
	// 	{
	// 		StatusCode = problemDetails.Status,
	// 	};
	// }

	// private static void HandleUnauthorizedAccessException(ExceptionContext context)
	// {
	// 	var problemDetails = new ProblemDetails
	// 	{
	// 		Title = "Access Denied",
	// 		Instance = context.HttpContext.Request.Path,
	// 		Status = StatusCodes.Status401Unauthorized,
	// 		Detail = context.Exception.Message,
	// 	};
	//
	// 	context.Result = new UnauthorizedObjectResult(problemDetails);
	// }

	// private static void HandleResourceNotFoundException(ExceptionContext context)
	// {
	// 	var problemDetails = new ProblemDetails
	// 	{
	// 		Title = "Resource Not Found",
	// 		Instance = context.HttpContext.Request.Path,
	// 		Status = StatusCodes.Status404NotFound,
	// 		Detail = context.Exception.Message,
	// 	};
	//
	// 	context.Result = new NotFoundObjectResult(problemDetails);
	// }

	// private static void HandleDomainValidationException(ExceptionContext context)
	// {
	// 	var problemDetails = new ValidationProblemDetails
	// 	{
	// 		// Title = "Validation Error",
	// 		Instance = context.HttpContext.Request.Path,
	// 		Status = StatusCodes.Status400BadRequest,
	// 		Detail = "Please refer to the errors property for additional details.",
	// 	};
	//
	// 	problemDetails.Errors.Add("DomainValidations", [context.Exception.Message,]);
	//
	// 	context.Result = new BadRequestObjectResult(problemDetails);
	// }

	// private static void HandleUndefinedExceptions(ExceptionContext context, IRequestContext requestContext)
	// {
	// 	var problemDetails = new ProblemDetails
	// 	{
	// 		Title = "Internal Server Error",
	// 		Instance = context.HttpContext.Request.Path,
	// 		Status = StatusCodes.Status500InternalServerError,
	// 		Detail = $"Internal Server Error. CorrelationId: '{requestContext.CorrelationId}'.",
	// 	};
	//
	// 	context.Result = new ObjectResult(problemDetails)
	// 	{
	// 		StatusCode = problemDetails.Status,
	// 	};
	// }
}
