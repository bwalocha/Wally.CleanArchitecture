using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Mediator;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using Wally.CleanArchitecture.MicroService.Application.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.PipelineBehaviours;

public class LogBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : IRequest<TResponse>
{
	private const int TimeoutMilliseconds = 500;
	
	private readonly ILogger<LogBehavior<TRequest, TResponse>> _logger;
	private readonly IRequestContext _requestContext;

	public LogBehavior(ILogger<LogBehavior<TRequest, TResponse>> logger, IRequestContext requestContext)
	{
		_logger = logger;
		_requestContext = requestContext;
	}

	public async ValueTask<TResponse> Handle(TRequest message, MessageHandlerDelegate<TRequest, TResponse> next, CancellationToken cancellationToken)
	{
		using var logContext =
			LogContext.PushProperty(nameof(IRequestContext.CorrelationId), _requestContext.CorrelationId);

		_logger.LogDebug(
			"Executing Request '{Request}' with Response '{Response}'",
			typeof(TRequest).Name, typeof(TResponse).Name);
		var stopWatch = Stopwatch.StartNew();

		try
		{
			return await next(message, cancellationToken);
		}
		finally
		{
			stopWatch.Stop();

			if (stopWatch.ElapsedMilliseconds > TimeoutMilliseconds)
			{
				_logger.LogWarning(
					"Request '{Request}' with Response '{Response}' executed in {ElapsedMilliseconds} ms",
					typeof(TRequest).Name, typeof(TResponse).Name, stopWatch.ElapsedMilliseconds);
			}
			else
			{
				_logger.LogDebug("Executed in {StopWatchElapsedMilliseconds} ms", stopWatch.ElapsedMilliseconds);
			}
		}
	}
}
