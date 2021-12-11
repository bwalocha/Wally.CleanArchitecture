using FluentValidation;

namespace Wally.CleanArchitecture.Contracts.Requests.User
{
	public class GetUsersRequestValidator : AbstractValidator<GetUsersRequest>
	{
		public GetUsersRequestValidator()
		{
			// RuleFor(x => x.Name).NotEmpty().MaximumLength(256);
		}
	}
}
