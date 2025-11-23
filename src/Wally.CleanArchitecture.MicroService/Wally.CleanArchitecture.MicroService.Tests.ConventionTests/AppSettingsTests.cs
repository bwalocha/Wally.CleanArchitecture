using AutoMapper.Internal;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Extensions;

namespace Wally.CleanArchitecture.MicroService.Tests.ConventionTests;

public class AppSettingsTests
{
	[Fact]
	public void AppSettings_Properties_ShouldHaveOnlyInitialSetters()
	{
		// Arrange
		IArchRule rule =
			PropertyMembers()
				.That()
				.AreDeclaredIn(Configuration.OtherTypes.AppSettings)
				.And()
				.HaveSetter()
				.As("AppSettings properties")
				.Should()
				.HaveInitOnlySetter();

		// Act

		// Assert
		rule.Check(Configuration.Architecture);
	}

	[Fact]
	public void AppSettings_ShouldHaveOnlyInitialSetters()
	{
		// Arrange
		var appSettingsTypes = Configuration.OtherTypes.AppSettings;

		// Act

		// Assert
		appSettingsTypes.ShouldSatisfyAllConditions(() =>
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
