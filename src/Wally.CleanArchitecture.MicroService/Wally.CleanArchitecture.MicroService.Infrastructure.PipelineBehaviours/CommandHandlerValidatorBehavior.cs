using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Mediator;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.PipelineBehaviours;

public class CommandHandlerValidatorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : Application.Abstractions.ICommand<TResponse>
{
	private readonly IValidator<TRequest> _validator;

	public CommandHandlerValidatorBehavior(IValidator<TRequest> validator)
	{
		_validator = validator;
	}

	public async ValueTask<TResponse> Handle(TRequest message, MessageHandlerDelegate<TRequest, TResponse> next, CancellationToken cancellationToken)
	{
		var validationResult = await _validator.ValidateAsync(message, cancellationToken);

		if (!validationResult.IsValid)
		{
			throw new ValidationException(validationResult.Errors);
		}

		return await next(message, cancellationToken);
	}
}
