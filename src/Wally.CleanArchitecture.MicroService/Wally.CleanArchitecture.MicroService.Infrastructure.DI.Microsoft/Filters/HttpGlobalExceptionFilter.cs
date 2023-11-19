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
using Wally.CleanArchitecture.MicroService.Domain.Abstractions;
using Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.Exceptions;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Filters;

public class HttpGlobalExceptionFilter : IExceptionFilter
{
	// private readonly IErrorResultProvider _errorResultProvider;
	private readonly ILogger<HttpGlobalExceptionFilter> _logger;

	public HttpGlobalExceptionFilter(
		// IErrorResultProvider errorResultProvider,
#pragma warning disable SA1114
		ILogger<HttpGlobalExceptionFilter> logger)
#pragma warning restore SA1114
	{
		// _errorResultProvider = errorResultProvider;
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
			case DomainException _:
				HandleDomainValidationException(context);

				break;
			case UnauthorizedAccessException _:
				HandleUnauthorizedAccessException(context);

				break;
			case ResourceNotFoundException _:
				HandleResourceNotFoundException(context);

				break;
			case UniqueConstraintException _:
			case CannotInsertNullException _:
			case MaxLengthExceededException _:
			case NumericOverflowException _:
			case ReferenceConstraintException _:
				// TODO: test it
				HandleSqlException(context);

				break;
			case DbUpdateException _:
				HandleSqlException(context);

				break;
			default:
				Debugger.Break();
				HandleUndefinedExceptions(context);

				break;
		}

		context.ExceptionHandled = true;
	}

	private void HandleSqlException(ExceptionContext context)
	{
		var problemDetails = new ValidationProblemDetails
		{
			Instance = context.HttpContext.Request.Path,
			Status = StatusCodes.Status409Conflict,
			Detail = "Please refer to the errors property for additional details.",
		};

		if (context.Exception.InnerException! is DbException exception)
		{
			problemDetails.Errors.Add(
				"Database",
				new[]
				{
					exception.ErrorCode.ToString(), exception.Message,
				});
			context.Result = new ConflictObjectResult(problemDetails);
		}
		else
		{
			context.Result = new ConflictObjectResult(problemDetails);
		}

		context.HttpContext.Response.StatusCode = StatusCodes.Status409Conflict;
	}

	private void HandleUnauthorizedAccessException(ExceptionContext context)
	{
		var problemDetails = new ValidationProblemDetails
		{
			Instance = context.HttpContext.Request.Path,
			Status = StatusCodes.Status401Unauthorized,
			Detail = context.Exception.Message,
		};

		context.Result = new UnauthorizedObjectResult(problemDetails);
		context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
	}

	private void HandleResourceNotFoundException(ExceptionContext context)
	{
		var problemDetails = new ValidationProblemDetails
		{
			Instance = context.HttpContext.Request.Path,
			Status = StatusCodes.Status404NotFound,
			Detail = context.Exception.Message,
		};

		context.Result = new NotFoundObjectResult(problemDetails);
		context.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
	}

	private void HandleDomainValidationException(ExceptionContext context)
	{
		var problemDetails = new ValidationProblemDetails
		{
			Instance = context.HttpContext.Request.Path,
			Status = StatusCodes.Status400BadRequest,
			Detail = "Please refer to the errors property for additional details.",
		};

		problemDetails.Errors.Add("DomainValidations", new[] { context.Exception.Message, });

		context.Result = new BadRequestObjectResult(problemDetails);
		context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
	}

	private static void HandleUndefinedExceptions(ExceptionContext context)
	{
		// var response = _errorResultProvider.GetResult(context);

		// context.Result = new InternalServerErrorObjectResult(response);
		context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
	}
}
