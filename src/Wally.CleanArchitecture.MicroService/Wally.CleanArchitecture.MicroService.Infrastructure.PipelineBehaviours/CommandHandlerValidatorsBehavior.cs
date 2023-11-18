using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Wally.Lib.DDD.Abstractions.Commands;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.PipelineBehaviours;

public class CommandHandlerValidatorsBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : ICommand, IRequest<TResponse>
{
	private readonly IEnumerable<IValidator<TRequest>> _validators;

	public CommandHandlerValidatorsBehavior(IEnumerable<IValidator<TRequest>> validators)
	{
		_validators = validators;
	}

	public async Task<TResponse> Handle(
		TRequest request,
		RequestHandlerDelegate<TResponse> next,
		CancellationToken cancellationToken)
	{
		if (!_validators.Any())
		{
			return await next();
		}

		var validationResults =
			await Task.WhenAll(_validators.Select(a => a.ValidateAsync(request, cancellationToken)));
		var validationErrors = validationResults.Where(a => !a.IsValid)
			.SelectMany(a => a.Errors)
			.ToArray();

		if (validationErrors.Any())
		{
			throw new ValidationException(validationErrors);
		}

		return await next();
	}
}
