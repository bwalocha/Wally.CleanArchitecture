using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Mediator;
using Wally.CleanArchitecture.MicroService.Application.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.PipelineBehaviours;

public class QueryHandlerValidatorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : Application.Abstractions.IQuery<TResponse>
	where TResponse : IResult
{
	private readonly IValidator<TRequest>? _validator;

	public QueryHandlerValidatorBehavior(IValidator<TRequest>? validator = null)
	{
		_validator = validator;
	}

	public async ValueTask<TResponse> Handle(TRequest message, MessageHandlerDelegate<TRequest, TResponse> next, CancellationToken cancellationToken)
	{
		if (_validator == null)
		{
			return await next(message, cancellationToken);
		}

		var validationResult = await _validator.ValidateAsync(message, cancellationToken);

		if (!validationResult.IsValid)
		{
			throw new ValidationException(validationResult.Errors);
		}

		return await next(message, cancellationToken);
	}
}
