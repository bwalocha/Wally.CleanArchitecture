using FluentValidation;

namespace Wally.CleanArchitecture.MicroService.Application.Contracts.Users.Requests;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
	public CreateUserRequestValidator()
	{
		RuleFor(a => a.Name)
			.NotEmpty()
			.MaximumLength(256);
	}
}
