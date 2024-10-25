using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using Wally.CleanArchitecture.MicroService.Application;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.PipelineBehaviours;

public class LogBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : IRequest<TResponse>
{
	private readonly ILogger<LogBehavior<TRequest, TResponse>> _logger;
	private readonly IRequestContext _requestContext;

	public LogBehavior(ILogger<LogBehavior<TRequest, TResponse>> logger, IRequestContext requestContext)
	{
		_logger = logger;
		_requestContext = requestContext;
	}

	public async Task<TResponse> Handle(
		TRequest request,
		RequestHandlerDelegate<TResponse> next,
		CancellationToken cancellationToken)
	{
		using var logContext = LogContext.PushProperty("CorrelationId", _requestContext.CorrelationId);

		_logger.LogInformation(
			"[{CorrelationId}] Executing request handler for request type: '{TypeofTRequestName}' and response type: '{TypeofTResponseName}'",
			_requestContext.CorrelationId, typeof(TRequest).Name, typeof(TResponse).Name);
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
				_logger.LogWarning("[{CorrelationId}] Executed in {StopWatchElapsedMilliseconds} ms",
					_requestContext.CorrelationId,
					stopWatch.ElapsedMilliseconds);
			}
			else
			{
				_logger.LogInformation("[{CorrelationId}] Executed in {StopWatchElapsedMilliseconds} ms",
					_requestContext.CorrelationId,
					stopWatch.ElapsedMilliseconds);
			}
		}
	}
}
