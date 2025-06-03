using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Wally.CleanArchitecture.MicroService.Application.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.PipelineBehaviours;

public class CommandHandlerValidatorsBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : ICommand<TResponse>
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
			return await next(cancellationToken);
		}

		var validationResults =
			await Task.WhenAll(_validators.Select(a => a.ValidateAsync(request, cancellationToken)));
		var validationErrors = validationResults.Where(a => !a.IsValid)
			.SelectMany(a => a.Errors)
			.ToArray();

		if (validationErrors.Length != 0)
		{
			throw new ValidationException(validationErrors);
		}

		return await next(cancellationToken);
	}
}
