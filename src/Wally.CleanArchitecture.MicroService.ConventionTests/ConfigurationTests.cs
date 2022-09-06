using System.Linq;

using FluentAssertions;

using Wally.CleanArchitecture.MicroService.ConventionTests.Helpers;

using Xunit;

namespace Wally.CleanArchitecture.MicroService.ConventionTests;

public class ConfigurationTests
{
	[Fact]
	public void Configuration_ShouldContainsAllAssemblies()
	{
		// Arrange
		var fromConfig = Configuration.Assemblies.GetAllAssemblies()
			.ToList();
		var fromInternal = TypeHelpers.GetAllInternalAssemblies()
			.ToList();

		// Act

		// Assert
		fromConfig.Should()
			.AllSatisfy(a => fromInternal.Contains(a));
	}
}
