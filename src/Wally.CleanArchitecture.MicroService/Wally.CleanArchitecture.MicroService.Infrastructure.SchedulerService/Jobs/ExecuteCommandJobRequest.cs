namespace Wally.CleanArchitecture.MicroService.Infrastructure.SchedulerService.Jobs;

public class ExecuteCommandJobRequest
{
	public required string CommandType { get; init; }
	public required object? Parameters { get; init; }
}
