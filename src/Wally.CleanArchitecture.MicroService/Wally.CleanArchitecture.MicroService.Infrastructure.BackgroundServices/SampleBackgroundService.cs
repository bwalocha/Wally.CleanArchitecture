using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Wally.CleanArchitecture.MicroService.Application.Users.Commands;
using Wally.CleanArchitecture.MicroService.Domain.Users;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.BackgroundServices;

public class SampleBackgroundService : BackgroundService
{
	private readonly ILogger<SampleBackgroundService> _logger;
	private readonly IServiceProvider _serviceProvider;

	public SampleBackgroundService(
		ILogger<SampleBackgroundService> logger,
		IServiceProvider serviceProvider)
	{
		_logger = logger;
		_serviceProvider = serviceProvider;
	}

	public override Task StartAsync(CancellationToken cancellationToken)
	{
		_logger.LogInformation("Starting...");

		return base.StartAsync(cancellationToken);
	}

	public override Task StopAsync(CancellationToken cancellationToken)
	{
		_logger.LogInformation("Stopping...");

		return base.StopAsync(cancellationToken);
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		// await SendCommandAsync(stoppingToken);
	}

	private async Task SendCommandAsync(CancellationToken cancellationToken)
	{
		using var scope = _serviceProvider.CreateScope();
		var sender = scope.ServiceProvider.GetRequiredService<ISender>();
		var command = new UpdateUserCommand(new UserId(Guid.Empty), string.Empty); // TODO:
		
		await sender.Send(command, cancellationToken);
	}
}
