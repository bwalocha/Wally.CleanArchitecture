using FluentValidation;
using Microsoft.FeatureManagement;
using Wally.CleanArchitecture.ApiGateway.Infrastructure.DI.Microsoft.Extensions;

namespace Wally.CleanArchitecture.ApiGateway.Infrastructure.DI.Microsoft.Models;

public class AppSettingsValidator : AbstractValidator<AppSettings>
{
	public AppSettingsValidator(IFeatureManager featureManager)
	{
		RuleFor(a => a.Authentication)
			.NotEmpty()
			.SetValidator(new AuthenticationSettingsValidator());
		if (!featureManager.IsEnabled(FeatureFlags.SwaggerDisabled))
		{
			RuleFor(a => a.SwaggerAuthentication)
				.NotEmpty()
				.SetValidator(new AuthenticationSettingsValidator());
		}
		
		RuleFor(a => a.Cors)
			.NotNull()
			.SetValidator(new CorsSettingsValidator());
		RuleFor(a => a.ReverseProxy)
			.NotNull()
			.SetValidator(new ReverseProxySettingsValidator());
	}
}
