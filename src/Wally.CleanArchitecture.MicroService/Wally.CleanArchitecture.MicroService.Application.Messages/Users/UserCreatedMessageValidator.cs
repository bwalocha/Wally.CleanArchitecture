using FluentValidation;
using Wally.CleanArchitecture.MicroService.Domain.Users;

namespace Wally.CleanArchitecture.MicroService.Application.Messages.Users;

public class UserCreatedMessageValidator : AbstractValidator<UserCreatedMessage>
{
	public UserCreatedMessageValidator()
	{
		RuleFor(a => a.Id)
			.NotEmpty();
		RuleFor(a => a.Name)
			.NotEmpty()
			.MaximumLength(User.MaxNameLength);
	}
}
