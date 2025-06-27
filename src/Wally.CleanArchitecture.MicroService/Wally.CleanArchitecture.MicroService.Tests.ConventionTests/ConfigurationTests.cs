using System.Linq;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Helpers;

namespace Wally.CleanArchitecture.MicroService.Tests.ConventionTests;

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
		fromConfig.ShouldSatisfyAllConditions(
			() => fromConfig.ForEach(a => fromInternal.ShouldContain(a)));
	}
}
