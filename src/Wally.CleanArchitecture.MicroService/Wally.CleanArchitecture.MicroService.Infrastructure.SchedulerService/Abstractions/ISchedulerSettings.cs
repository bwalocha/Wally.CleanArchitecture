namespace Wally.CleanArchitecture.MicroService.Infrastructure.SchedulerService.Abstractions;

public interface ISchedulerSettings
{
	string BasePath { get; }
	public string? BackendDomain { get; }
	public int MaxConcurrency { get; }
}
