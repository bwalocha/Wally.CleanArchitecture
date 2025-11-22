using Microsoft.Extensions.Logging;
using TickerQ.Utilities.Enums;
using TickerQ.Utilities.Interfaces;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.SchedulerService.Handlers;

public class JobExceptionHandler : ITickerExceptionHandler
{
	private readonly ILogger<JobExceptionHandler> _logger;

	public JobExceptionHandler(ILogger<JobExceptionHandler> logger)
	{
		_logger = logger;
	}

	public Task HandleExceptionAsync(
		Exception exception,
		Guid tickerId,
		TickerType tickerType)
	{
		_logger.LogError(exception, "Job {TickerId} ({TickerType}) failed", tickerId, tickerType);

		return Task.CompletedTask;
	}

	public Task HandleCanceledExceptionAsync(Exception exception, Guid tickerId, TickerType tickerType)
	{
		_logger.LogWarning("Job {TickerId} ({TickerType}) was cancelled", tickerId, tickerType);

		return Task.CompletedTask;
	}
}
