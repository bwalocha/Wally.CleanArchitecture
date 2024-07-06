using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Extensions;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Helpers;
using Wally.Lib.DDD.Abstractions.Queries;
using Xunit;

namespace Wally.CleanArchitecture.MicroService.Tests.ConventionTests;

public class QueryTests
{
	[Fact]
	public void Application_Query_ShouldNotExposeSetter()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.ImplementsGenericInterface(typeof(IQuery<>)));

		types
			.Types()
			.Properties()
			.Should()
			.NotBeWritable("query should be immutable");
	}

	[Fact]
	public void Application_Query_ShouldBeExcludedFromCodeCoverage()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.ImplementsGenericInterface(typeof(IQuery<>)))
			.Where(a => a.IsClass);

		types
			.Types()
			.Should()
			.BeDecoratedWith<ExcludeFromCodeCoverageAttribute>();
	}

	[Fact]
	public void Application_Query_ShouldBeSealed()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.ImplementsGenericInterface(typeof(IQuery<>)))
			.Where(a => a.IsClass);

		types
			.Types()
			.Should()
			.BeSealed();
	}

	[Fact]
	public void Application_Query_ShouldHaveCorrespondingHandler()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();

		using (new AssertionScope(new AssertionStrategy()))
		{
			foreach (var assembly in assemblies)
			{
				foreach (var type in assembly.GetTypes()
							.Where(a => a.ImplementsGenericInterface(typeof(IQuery<>)))
							.Where(a => a.IsClass)
							.Types())
				{
					assemblies.SelectMany(a => a.GetTypes())
						.SingleOrDefault(a => a.Name == $"{type.Name}Handler")
						.Should()
						.NotBeNull("Query '{0}' should have corresponding Handler", type);
				}
			}
		}
	}
}
