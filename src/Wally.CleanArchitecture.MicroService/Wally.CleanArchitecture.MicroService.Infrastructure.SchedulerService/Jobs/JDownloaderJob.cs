using Mediator;
using TickerQ.Utilities.Base;
using TickerQ.Utilities.Entities;
using TickerQ.Utilities.Interfaces.Managers;
using Wally.CleanArchitecture.MicroService.Application.Users.Commands;
using Wally.CleanArchitecture.MicroService.Domain.Users;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.SchedulerService.Jobs;

public class JDownloaderJob
{
	private readonly ITimeTickerManager<TimeTickerEntity> _timeTickerManager;
	private readonly IMediator _mediator;

	public JDownloaderJob(ITimeTickerManager<TimeTickerEntity> timeTickerManager, IMediator mediator)
	{
		_timeTickerManager = timeTickerManager;
		_mediator = mediator;
	}

	public async Task ScheduleJob()
	{
		var result = await _timeTickerManager.AddAsync(new TimeTickerEntity
		{
			Function = "HelloWorld",
			ExecutionTime = DateTime.UtcNow.AddSeconds(10) // Run in 10 seconds
		});

		if (result.IsSucceeded)
		{
			Console.WriteLine($"Job scheduled! ID: {result.Result.Id}");
		}
	}

	[TickerFunction("CleanupOldNotifications")]
	public async Task ExecuteJobAsync(
		TickerFunctionContext context,
		CancellationToken cancellationToken)
	{
		var command = new UpdateUserCommand(new UserId(Guid.NewGuid()), "testUser");

		await _mediator.Send(command, cancellationToken);
	}
}
