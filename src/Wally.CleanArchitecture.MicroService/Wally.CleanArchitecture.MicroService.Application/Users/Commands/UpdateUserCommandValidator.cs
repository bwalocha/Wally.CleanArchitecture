using FluentValidation;
using Wally.CleanArchitecture.MicroService.Domain.Users;

namespace Wally.CleanArchitecture.MicroService.Application.Users.Commands;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
	public UpdateUserCommandValidator()
	{
		RuleFor(a => a.UserId)
			.NotEmpty();

		RuleFor(a => a.Name)
			.NotEmpty()
			.MaximumLength(User.NameMaxLength);
	}
}
