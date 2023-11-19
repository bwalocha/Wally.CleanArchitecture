using System.Linq;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Models;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Extensions;

public static class OptionsExtensions
{
	public static IServiceCollection AddOptions(this IServiceCollection services, AppSettings settings)
	{
		services.AddValidatorsFromAssemblyContaining<IInfrastructureDIMicrosoftAssemblyMarker>(
			ServiceLifetime.Transient);
		services.AddOptions<AppSettings>()
			.BindConfiguration(string.Empty)
			.ValidateFluently()
			.ValidateOnStart();

		return services;
	}

	private static OptionsBuilder<TOptions> ValidateFluently<TOptions>(this OptionsBuilder<TOptions> builder)
		where TOptions : class
	{
		builder.Services.AddSingleton<IValidateOptions<TOptions>>(s =>
			new ValidateOptions<TOptions>(builder.Name, s.GetRequiredService<IValidator<TOptions>>()));

		return builder;
	}

	private class ValidateOptions<TOptions> : IValidateOptions<TOptions>
		where TOptions : class
	{
		private readonly string? _name;
		private readonly IValidator<TOptions> _validator;

		public ValidateOptions(string? name, IValidator<TOptions> validator)
		{
			_name = name;
			_validator = validator;
		}

		public ValidateOptionsResult Validate(string? name, TOptions options)
		{
			// Null name is used to configure all named options.
			if (_name != null && _name != name)
			{
				// Ignore if not validating this instance.
				return ValidateOptionsResult.Skip;
			}

			var result = _validator.Validate(options);
			if (result.IsValid)
			{
				return ValidateOptionsResult.Success;
			}

			var errors =
				result.Errors.Select(a => $"Options validation failed for '{a.PropertyName}': {a.ErrorMessage}");
			return ValidateOptionsResult.Fail(errors);
		}
	}
}
