using System.Threading;
using System.Threading.Tasks;

using FluentValidation;

using MediatR;

using Wally.Lib.DDD.Abstractions.Responses;

namespace Wally.CleanArchitecture.MicroService.PipelineBehaviours;

public class QueryHandlerValidatorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : IRequest<TResponse> where TResponse : IResponse
{
	private readonly IValidator<TRequest>? _validator;

	public QueryHandlerValidatorBehavior(IValidator<TRequest>? validator = null)
	{
		_validator = validator;
	}

	public async Task<TResponse> Handle(
		TRequest request,
		CancellationToken cancellationToken,
		RequestHandlerDelegate<TResponse> next)
	{
		if (_validator != null)
		{
			var validationResult = await _validator.ValidateAsync(request, cancellationToken);

			if (!validationResult.IsValid)
			{
				throw new ValidationException(validationResult.Errors);
			}
		}

		return await next();
	}
}
