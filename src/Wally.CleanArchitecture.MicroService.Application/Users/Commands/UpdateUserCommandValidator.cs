using System;

using FluentValidation;

namespace Wally.CleanArchitecture.MicroService.Application.Users.Commands;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
	public UpdateUserCommandValidator()
	{
		RuleFor(a => a.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty);
		RuleFor(a => a.Name)
			.NotEmpty()
			.MinimumLength(1)
			.MaximumLength(256);
	}
}
