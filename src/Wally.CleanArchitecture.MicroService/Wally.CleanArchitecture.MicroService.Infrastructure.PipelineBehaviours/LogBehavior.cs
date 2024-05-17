using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using Wally.CleanArchitecture.MicroService.Domain;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.PipelineBehaviours;

public class LogBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : IRequest<TResponse>
{
	private readonly ILogger<LogBehavior<TRequest, TResponse>> _logger;
	
	public LogBehavior(ILogger<LogBehavior<TRequest, TResponse>> logger)
	{
		_logger = logger;
	}
	
	public async Task<TResponse> Handle(
		TRequest request,
		RequestHandlerDelegate<TResponse> next,
		CancellationToken cancellationToken)
	{
		var correlationId = GetCorrelationId();
		
		using var logContext = LogContext.PushProperty("CorrelationId", correlationId);
		
		_logger.LogInformation(
			"[{CorrelationId}] Executing request handler for request type: '{TypeofTRequestName}' and response type: '{TypeofTResponseName}'",
			correlationId, typeof(TRequest).Name, typeof(TResponse).Name);
		var stopWatch = Stopwatch.StartNew();
		try
		{
			return await next();
		}
		finally
		{
			stopWatch.Stop();
			
			if (stopWatch.ElapsedMilliseconds > 500)
			{
				_logger.LogWarning("[{CorrelationId}] Executed in {StopWatchElapsedMilliseconds} ms", correlationId,
					stopWatch.ElapsedMilliseconds);
			}
			else
			{
				_logger.LogInformation("[{CorrelationId}] Executed in {StopWatchElapsedMilliseconds} ms", correlationId,
					stopWatch.ElapsedMilliseconds);
			}
		}
	}
	
	private CorrelationId GetCorrelationId()
	{
		// const string CorrelationIdHeaderName = "X-Correlation-Id";
		// TODO: use HttpContext or CorrelationId from MassTransit Message
		// ...
		
		return new CorrelationId(Guid.NewGuid());
	}
}
