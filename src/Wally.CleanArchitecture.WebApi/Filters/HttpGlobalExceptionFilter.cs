using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Wally.CleanArchitecture.Persistence.Exceptions;

namespace Wally.CleanArchitecture.WebApi.Filters
{
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
				/*case DomainValidationException _:
					HandleDomainValidationException(context);

					break;*/
				case UnauthorizedAccessException _:
					HandleUnauthorizedAccessException(context);

					break;
				case ResourceNotFoundException _:
					HandleResourceNotFoundException(context);

					break;
				default:
					Debugger.Break();
					HandleUndefinedExceptions(context);

					break;
			}

			context.ExceptionHandled = true;
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

		private void HandleUndefinedExceptions(ExceptionContext context)
		{
			// var response = _errorResultProvider.GetResult(context);

			// context.Result = new InternalServerErrorObjectResult(response);
			context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
		}
	}
}
