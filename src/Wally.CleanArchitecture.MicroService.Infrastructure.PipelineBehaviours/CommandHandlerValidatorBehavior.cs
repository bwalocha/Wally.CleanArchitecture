using System.Threading;
using System.Threading.Tasks;

using FluentValidation;

using MediatR;

using Wally.Lib.DDD.Abstractions.Commands;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.PipelineBehaviours;

public class CommandHandlerValidatorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : ICommand, IRequest<TResponse>
{
	private readonly IValidator<TRequest> _validator;

	public CommandHandlerValidatorBehavior(IValidator<TRequest> validator)
	{
		_validator = validator;
	}

	public async Task<TResponse> Handle(
		TRequest request,
		RequestHandlerDelegate<TResponse> next,
		CancellationToken cancellationToken)
	{
		var validationResult = await _validator.ValidateAsync(request, cancellationToken);

		if (!validationResult.IsValid)
		{
			throw new ValidationException(validationResult.Errors);
		}

		return await next();
	}
}
