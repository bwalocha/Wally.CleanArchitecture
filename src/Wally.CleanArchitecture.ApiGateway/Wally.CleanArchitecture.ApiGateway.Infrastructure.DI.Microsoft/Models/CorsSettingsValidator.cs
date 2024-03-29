using FluentValidation;

namespace Wally.CleanArchitecture.ApiGateway.Infrastructure.DI.Microsoft.Models;

public class CorsSettingsValidator : AbstractValidator<CorsSettings>
{
	public CorsSettingsValidator()
	{
		RuleForEach(a => a.Origins)
			.NotEmpty();
	}
}
