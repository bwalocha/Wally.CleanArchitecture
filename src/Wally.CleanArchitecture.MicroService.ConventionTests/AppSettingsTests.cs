using System.Linq;

using AutoMapper.Internal;

using FluentAssertions;
using FluentAssertions.Execution;

using Wally.CleanArchitecture.MicroService.ConventionTests.Helpers;

using Xunit;

namespace Wally.CleanArchitecture.MicroService.ConventionTests;

public class AppSettingsTests
{
	[Fact]
	public void AppSettings_ShouldHaveOnlyInitialSetters()
	{
		// Arrange
		var appSettingsTypes = Configuration.Types.AppSettings.ToList();

		// Act

		// Assert
		using (new AssertionScope(new AssertionStrategy()))
		{
			foreach (var appSettingsType in appSettingsTypes)
			{
				foreach (var property in appSettingsType.Properties())
				{
					if (property.CanBeSet())
					{
						property.IsInitOnly()
							.Should()
							.BeTrue("AppSettings type '{0}' should not expose setter '{1}'", appSettingsType, property);
					}
					else
					{
						property.DeclaringType!.IsClass.Should()
							.BeTrue();
					}
				}
			}
		}
	}
}
