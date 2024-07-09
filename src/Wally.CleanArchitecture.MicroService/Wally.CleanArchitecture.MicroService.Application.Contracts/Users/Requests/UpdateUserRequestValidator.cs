using FluentValidation;

namespace Wally.CleanArchitecture.MicroService.Application.Contracts.Users.Requests;

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
	public UpdateUserRequestValidator()
	{
		RuleFor(a => a.Name)
			.NotEmpty()
			.MaximumLength(256);
	}
}
