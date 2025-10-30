using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Wally.CleanArchitecture.MicroService.Domain.Abstractions;
// using Wally.CleanArchitecture.MicroService.Application.Abstractions;

namespace Wally.CleanArchitecture.MicroService.WebApi.Handlers;

internal sealed class ValidationExceptionHandler : IExceptionHandler
{
	private readonly ILogger<ValidationExceptionHandler> _logger;
	private readonly IProblemDetailsService _problemDetailsService;
	// private readonly IRequestContext _requestContext;

	public ValidationExceptionHandler(IProblemDetailsService problemDetailsService, /*IRequestContext requestContext,*/ ILogger<ValidationExceptionHandler> logger)
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
			case DomainException _:
			case FluentValidation.ValidationException _:
				httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
				return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
				{
					HttpContext = httpContext,
					Exception = exception,
					ProblemDetails = new ProblemDetails
					{
						Type = exception.GetType()
							.Name,
						Title = "Validation Exception",
						Status = StatusCodes.Status400BadRequest,
						Instance = httpContext.Request.Path,
						Detail = exception.Message,
						// Extensions = exception is FluentValidation.ValidationException validationException
						// 	? validationException.Errors.ToDictionary(a => a.PropertyName, a => (object?)a.ErrorMessage) 
						// 	: new Dictionary<string, object?>(),
					},
					// AdditionalMetadata = 
				});
			default:
				return false;
		}
	}
}
