using FluentValidation;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Models;

public class AppSettingsValidator : AbstractValidator<AppSettings>
{
	public AppSettingsValidator()
	{
		// TODO:
		/*RuleFor(a => a.Authentication)
			.NotEmpty()
			.SetValidator(new AuthenticationSettingsValidator());*/
		// TODO:
		/*RuleFor(a => a.SwaggerAuthentication)
			.NotEmpty()
			.SetValidator(new AuthenticationSettingsValidator());*/
		RuleFor(a => a.Database)
			.NotEmpty()
			.SetValidator(new DbContextSettingsValidator());
		RuleFor(a => a.ConnectionStrings)
			.NotEmpty()
			.SetValidator(new ConnectionStringsValidator());
		RuleFor(a => a.MessageBroker)
			.NotEmpty()
			.NotEqual(MessageBrokerType.Unknown);
	}
}
