using Mediator;
using Microsoft.Extensions.Logging;
using TickerQ.Utilities.Base;
// using TickerQ.Utilities.Entities;
// using TickerQ.Utilities.Interfaces.Managers;
using Wally.CleanArchitecture.MicroService.Application.Users.Commands;
using Wally.CleanArchitecture.MicroService.Domain.Users;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.SchedulerService.Jobs;

public class SampleJob
{
	private readonly ILogger<SampleJob> _logger;

	// private readonly ITimeTickerManager<TimeTickerEntity> _timeTickerManager;
	private readonly IMediator _mediator;
	// private readonly TimeProvider _timeProvider;

	public SampleJob(
		// ITimeTickerManager<TimeTickerEntity> timeTickerManager,
		IMediator mediator,
		// TimeProvider timeProvider
		ILogger<SampleJob> logger
		)
	{
		_logger = logger;
		// _timeTickerManager = timeTickerManager;
		_mediator = mediator;
		// _timeProvider = timeProvider;
	}

	[TickerFunction("ExecuteJob", "0/10 * * * * *")]
	public async Task ExecuteJobAsync(
		TickerFunctionContext context,
		CancellationToken cancellationToken)
	{
		_logger.LogInformation("ExecuteJobAsync");
		
		var command = new UpdateUserCommand(new UserId(Guid.NewGuid()), "testUser");

		await _mediator.Send(command, cancellationToken);
	}
}
