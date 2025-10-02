using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Mediator;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.PipelineBehaviours;

public class CommandHandlerValidatorsBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : Application.Abstractions.ICommand<TResponse>
{
	private readonly IEnumerable<IValidator<TRequest>> _validators;

	public CommandHandlerValidatorsBehavior(IEnumerable<IValidator<TRequest>> validators)
	{
		_validators = validators;
	}

	public async ValueTask<TResponse> Handle(TRequest message, MessageHandlerDelegate<TRequest, TResponse> next, CancellationToken cancellationToken)
	{
		if (!_validators.Any())
		{
			return await next(message, cancellationToken);
		}

		var validationResults =
			await Task.WhenAll(_validators.Select(a => a.ValidateAsync(message, cancellationToken)));
		var validationErrors = validationResults.Where(a => !a.IsValid)
			.SelectMany(a => a.Errors)
			.ToArray();

		if (validationErrors.Length != 0)
		{
			throw new ValidationException(validationErrors);
		}

		return await next(message, cancellationToken);
	}
}
