using FluentValidation;
using Wally.CleanArchitecture.MicroService.Domain.Users;

namespace Wally.CleanArchitecture.MicroService.WebApi.Requests;

public class GetUsersRequestValidator : AbstractValidator<GetUsersRequest>
{
	public GetUsersRequestValidator()
	{
		RuleFor(a => a.Id)
			.NotEmpty()
			.When(a => a.Id != null);

		RuleFor(a => a.Name)
			.NotEmpty()
			.MaximumLength(User.NameMaxLength)
			.When(a => a.Name != null);
	}
}
