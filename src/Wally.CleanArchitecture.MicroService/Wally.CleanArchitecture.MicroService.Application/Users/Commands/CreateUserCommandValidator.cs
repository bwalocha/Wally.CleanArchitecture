using FluentValidation;

namespace Wally.CleanArchitecture.MicroService.Application.Users.Commands;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
	public CreateUserCommandValidator()
	{
		RuleFor(a => a.UserId)
			.NotEmpty();
		
		RuleFor(a => a.Name)
			.NotEmpty()
			.MaximumLength(256);
	}
}
