using FluentValidation;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.Abstractions.Models;

public class ConnectionStringsValidator : AbstractValidator<ConnectionStrings>
{
	public ConnectionStringsValidator()
	{
		RuleFor(a => a.Database)
			.NotEmpty();
		RuleFor(a => a.ServiceBus)
			.NotEmpty();
	}
}
