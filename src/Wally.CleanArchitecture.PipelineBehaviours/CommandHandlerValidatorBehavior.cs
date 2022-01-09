using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Wally.Lib.DDD.Abstractions.Commands;
// using Wally.Lib.DDD.Abstractions.Responses;

namespace Wally.CleanArchitecture.PipelineBehaviours
{
	public class CommandHandlerValidatorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
		where TRequest : ICommand, MediatR.IRequest<TResponse>
		// where TResponse : IResponse
	{
		private readonly IValidator<TRequest> _validator;

		public CommandHandlerValidatorBehavior(IValidator<TRequest> validator)
		{
			_validator = validator;
		}

		public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
		{
			var validationResult = await _validator.ValidateAsync(request, cancellationToken);
			
			if (!validationResult.IsValid)
			{
				throw new ValidationException(validationResult.Errors);
			}

			return await next();
		}
	}
}
