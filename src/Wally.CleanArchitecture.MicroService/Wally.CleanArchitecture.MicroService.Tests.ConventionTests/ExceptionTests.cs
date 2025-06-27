using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Extensions;

namespace Wally.CleanArchitecture.MicroService.Tests.ConventionTests;

public class ExceptionTests
{
	[Fact]
	public void Application_Exception_ShouldBeExcludedFromCodeCoverage()
	{
		var applicationTypes = Configuration.Assemblies.Application.GetAllTypes();
		var types = applicationTypes
			.Where(a => a == typeof(Exception));

		types.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in types)
			{
				type.ShouldBeDecoratedWith<ExcludeFromCodeCoverageAttribute>();
			}
		});
	}
}
