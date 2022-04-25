using FluentValidation;

namespace Wally.CleanArchitecture.MicroService.Contracts.Requests.Users;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
	public CreateUserRequestValidator()
	{
		RuleFor(a => a.Name)
			.NotEmpty()
			.MinimumLength(1)
			.MaximumLength(256);
	}
}
