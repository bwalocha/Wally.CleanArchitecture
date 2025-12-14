using Wally.CleanArchitecture.MicroService.Infrastructure.SchedulerService.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Models;

public class SchedulerSettings : ISchedulerSettings
{
	public string BasePath { get; init; } = "/scheduler";
	public string? BackendDomain { get; init; }
	public int MaxConcurrency { get; init; } = 1;
}
