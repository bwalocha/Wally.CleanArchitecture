using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Extensions;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Helpers;
using Xunit;

namespace Wally.CleanArchitecture.MicroService.Tests.ConventionTests;

public class ExceptionTests
{
	[Fact]
	public void Application_Exception_ShouldBeExcludedFromCodeCoverage()
	{
		var applicationTypes = Configuration.Assemblies.Application.GetAllTypes();
		var types = applicationTypes
			.Where(a => a == typeof(Exception));

		using (new AssertionScope(new AssertionStrategy()))
		{
			foreach (var type in types)
			{
				type
					.Should()
					.BeDecoratedWith<ExcludeFromCodeCoverageAttribute>();
			}
		}
	}
}
