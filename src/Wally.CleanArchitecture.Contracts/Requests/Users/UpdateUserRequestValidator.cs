using FluentValidation;

namespace Wally.CleanArchitecture.Contracts.Requests.Users;

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
	public UpdateUserRequestValidator()
	{
		RuleFor(x => x.Name)
			.NotEmpty()
			.MaximumLength(256);
	}
}
