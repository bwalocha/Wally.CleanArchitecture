using System;
using System.Data.Common;
using System.Diagnostics;
using System.Threading.Tasks;
using EntityFramework.Exceptions.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wally.CleanArchitecture.MicroService.Application.Abstractions;
using Wally.CleanArchitecture.MicroService.Domain.Abstractions;
using Wally.CleanArchitecture.MicroService.Domain.Exceptions;
using Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Filters;

public class HttpGlobalExceptionFilter : IExceptionFilter
{
	private readonly ILogger<HttpGlobalExceptionFilter> _logger;
	private readonly IRequestContext _requestContext;

	public HttpGlobalExceptionFilter(IRequestContext requestContext, ILogger<HttpGlobalExceptionFilter> logger)
	{
		_requestContext = requestContext;
		_logger = logger;
	}

	public void OnException(ExceptionContext context)
	{
		if (context.Exception is TaskCanceledException)
		{
			return;
		}

		_logger.LogError(new EventId(context.Exception.HResult), context.Exception, context.Exception.Message);

		switch (context.Exception)
		{
			case PermissionDeniedException _:
				HandlePermissionDeniedException(context);
				break;
			case DomainException _:
			case FluentValidation.ValidationException _:
				HandleDomainValidationException(context);
				break;
			case UnauthorizedAccessException _:
				HandleUnauthorizedAccessException(context);
				break;
			case INotFound _:
				HandleResourceNotFoundException(context);
				break;
			case UniqueConstraintException _:
			case CannotInsertNullException _:
			case MaxLengthExceededException _:
			case NumericOverflowException _:
			case ReferenceConstraintException _:
				HandleSqlException(context);
				break;
			case DbUpdateException _:
				HandleSqlException(context);
				break;
			default:
				HandleUndefinedExceptions(context, _requestContext);
				Debugger.Break();
				break;
		}

		context.ExceptionHandled = true;
	}

	private static void HandleSqlException(ExceptionContext context)
	{
		var problemDetails = new ValidationProblemDetails
		{
			// Title = "Validation Error",
			Instance = context.HttpContext.Request.Path,
			Status = StatusCodes.Status409Conflict,
			Detail = "Please refer to the errors property for additional details.",
		};

		if (context.Exception.InnerException! is DbException exception)
		{
			problemDetails.Errors.Add(
				"Database",
				[
					exception.ErrorCode.ToString(), exception.Message,
				]);
		}

		context.Result = new ConflictObjectResult(problemDetails);
	}

	private static void HandlePermissionDeniedException(ExceptionContext context)
	{
		var problemDetails = new ProblemDetails
		{
			Title = "Permission Denied",
			Instance = context.HttpContext.Request.Path,
			Status = StatusCodes.Status403Forbidden,
			Detail = context.Exception.Message,
		};

		context.Result = new ObjectResult(problemDetails)
		{
			StatusCode = problemDetails.Status,
		};
	}

	private static void HandleUnauthorizedAccessException(ExceptionContext context)
	{
		var problemDetails = new ProblemDetails
		{
			Title = "Access Denied",
			Instance = context.HttpContext.Request.Path,
			Status = StatusCodes.Status401Unauthorized,
			Detail = context.Exception.Message,
		};

		context.Result = new UnauthorizedObjectResult(problemDetails);
	}

	private static void HandleResourceNotFoundException(ExceptionContext context)
	{
		var problemDetails = new ProblemDetails
		{
			Title = "Resource Not Found",
			Instance = context.HttpContext.Request.Path,
			Status = StatusCodes.Status404NotFound,
			Detail = context.Exception.Message,
		};

		context.Result = new NotFoundObjectResult(problemDetails);
	}

	private static void HandleDomainValidationException(ExceptionContext context)
	{
		var problemDetails = new ValidationProblemDetails
		{
			// Title = "Validation Error",
			Instance = context.HttpContext.Request.Path,
			Status = StatusCodes.Status400BadRequest,
			Detail = "Please refer to the errors property for additional details.",
		};

		problemDetails.Errors.Add("DomainValidations", [context.Exception.Message,]);

		context.Result = new BadRequestObjectResult(problemDetails);
	}

	private static void HandleUndefinedExceptions(ExceptionContext context, IRequestContext requestContext)
	{
		var problemDetails = new ProblemDetails
		{
			Title = "Internal Server Error",
			Instance = context.HttpContext.Request.Path,
			Status = StatusCodes.Status500InternalServerError,
			Detail = $"Internal Server Error. CorrelationId: '{requestContext.CorrelationId}'.",
		};

		context.Result = new ObjectResult(problemDetails)
		{
			StatusCode = problemDetails.Status,
		};
	}
}
