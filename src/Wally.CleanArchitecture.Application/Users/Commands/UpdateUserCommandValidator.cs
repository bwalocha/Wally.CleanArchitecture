using FluentValidation;

namespace Wally.CleanArchitecture.Application.Users.Commands
{
	public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
	{
		public UpdateUserCommandValidator()
		{
			RuleFor(a => a.Id).NotEmpty();
			RuleFor(a => a.Name).NotEmpty().MaximumLength(256);
		}
	}
}
