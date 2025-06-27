using AutoMapper.Internal;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Extensions;

namespace Wally.CleanArchitecture.MicroService.Tests.ConventionTests;

public class AppSettingsTests
{
	[Fact]
	public void AppSettings_ShouldHaveOnlyInitialSetters()
	{
		// Arrange
		var appSettingsTypes = Configuration.Types.AppSettings;

		// Act

		// Assert
		appSettingsTypes.ShouldSatisfyAllConditions(
			() =>
			{
				foreach (var appSettingsType in appSettingsTypes)
				{
					foreach (var property in appSettingsType.GetProperties())
					{
						if (property.CanBeSet())
						{
							property.IsInitOnly()
								.ShouldBeTrue(
									$"AppSettings type '{appSettingsType}' should not expose setter '{property}'");
						}
						else
						{
							property.DeclaringType!.IsClass
								.ShouldBeTrue();
						}
					}
				}
			});
	}
}
