using System;
using System.Collections.Generic;
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
		if (!TryMap(exception, out var error))
		{
			return false;
		}

		var (constraint, table, props) = ExtractConstraintInfo(exception);

		_logger.LogError(exception,
			"Database error at {Path}. Type: {Type}, Constraint: {Constraint}, Table: {Table}, Properties: {@Props}",
			httpContext.Request.Path,
			exception.GetType().Name,
			constraint,
			table,
			props);

		var extensions = new Dictionary<string, object?>
		{
			["traceId"] = httpContext.TraceIdentifier,
		};

		httpContext.Response.StatusCode = error.Status;

		return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
		{
			HttpContext = httpContext,
			Exception = exception,
			ProblemDetails = new ProblemDetails
			{
				Type = error.Code,
				Title = error.Title,
				Status = error.Status,
				Instance = httpContext.Request.Path,
				Extensions = extensions,
			},
		});
	}

	private static bool TryMap(Exception exception, out ApiError error)
	{
		switch (exception)
		{
			case UniqueConstraintException:
				error = new ApiError("unique_constraint", "Unique constraint violation", 409);
				return true;

			case CannotInsertNullException:
				error = new ApiError("null_violation", "Null value not allowed", 400);
				return true;

			case MaxLengthExceededException:
				error = new ApiError("max_length_exceeded", "Max length exceeded", 400);
				return true;

			case NumericOverflowException:
				error = new ApiError("numeric_overflow", "Numeric overflow", 400);
				return true;

			case ReferenceConstraintException:
				error = new ApiError("reference_constraint", "Reference constraint violation", 409);
				return true;

			case DbUpdateException:
				error = new ApiError("db_update_error", "Database update failed", 409);
				return true;

			default:
				error = default!;
				return false;
		}
	}
	
	private static (string? constraint, string? table, IReadOnlyList<string>? props) ExtractConstraintInfo(Exception ex)
	{
		return ex switch
		{
			UniqueConstraintException u => (u.ConstraintName, u.SchemaQualifiedTableName, u.ConstraintProperties),
			ReferenceConstraintException r => (r.ConstraintName, r.SchemaQualifiedTableName, r.ConstraintProperties),
			_ => (null, null, null),
		};
	}

	private sealed record ApiError(
		string Code,
		string Title,
		int Status);
}
