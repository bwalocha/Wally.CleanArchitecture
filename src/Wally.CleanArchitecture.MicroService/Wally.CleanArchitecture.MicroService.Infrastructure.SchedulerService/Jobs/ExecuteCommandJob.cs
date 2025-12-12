using System.Linq;
using System.Text.Json;
using Mediator;
using Microsoft.Extensions.Logging;
using TickerQ.Utilities.Base;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.SchedulerService.Jobs;

public class ExecuteCommandJob
{
	private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
	{
		PropertyNameCaseInsensitive = true,
		Converters = { new TypeConverterJsonConverterFactory(), },
	};
	private readonly ILogger<ExecuteCommandJob> _logger;

	private readonly IMediator _mediator;

	public ExecuteCommandJob(
		IMediator mediator,
		ILogger<ExecuteCommandJob> logger)
	{
		_logger = logger;
		_mediator = mediator;
	}

	[TickerFunction(nameof(ExecuteCommandJob))]
	public async Task ExecuteCommandJobAsync(
		TickerFunctionContext<ExecuteCommandJobRequest> context,
		CancellationToken cancellationToken)
	{
		_logger.LogInformation("ExecuteCommandJobAsync: {@Request}", context.Request);

		var command = CreateCommandInstance(context.Request.CommandType, (JsonElement?)context.Request.Parameters);

		await _mediator.Send(command, cancellationToken);
	}

	private static Wally.CleanArchitecture.MicroService.Application.Abstractions.ICommand CreateCommandInstance(
		string commandType, JsonElement? jsonParams)
	{
		var type = Type.GetType(commandType);
		if (type == null)
		{
			throw new InvalidOperationException($"Unknown Command Type: {commandType}");
		}

		var constructors = type.GetConstructors();
		foreach (var ctor in constructors)
		{
			var ctorParams = ctor.GetParameters();

			if (!ctorParams.All(p => jsonParams.HasValue && jsonParams.Value.TryGetProperty(p.Name!, out _)))
			{
				continue;
			}

			var args = ctorParams
				.Select(p =>
				{
					var prop = jsonParams!.Value.GetProperty(p.Name!);

					return JsonSerializer.Deserialize(prop.GetRawText(), p.ParameterType, JsonSerializerOptions);
				})
				.ToArray();

			return (Wally.CleanArchitecture.MicroService.Application.Abstractions.ICommand)ctor.Invoke(args);
		}

		throw new InvalidOperationException(
			$"Unknown Constructor for Command: {commandType} and Parameters {jsonParams}");
	}
}
