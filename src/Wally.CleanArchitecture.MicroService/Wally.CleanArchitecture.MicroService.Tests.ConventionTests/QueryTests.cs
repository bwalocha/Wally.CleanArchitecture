using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Shouldly;
using Wally.CleanArchitecture.MicroService.Application.Abstractions;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Extensions;
using Xunit;

namespace Wally.CleanArchitecture.MicroService.Tests.ConventionTests;

public class QueryTests
{
	[Fact]
	public void Application_Query_ShouldEndsWithQuery()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.ImplementsGenericInterface(typeof(IQuery<>)))
			.Where(a => a != typeof(IQuery<>))
			.Where(a => a != typeof(PagedQuery<,>));

		types.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in types)
			{
				type.Name
					.ShouldEndWith("Query", Case.Sensitive, $"Type '{type}' name should end with 'Query'");
			}
		});
	}

	[Fact]
	public void Application_Query_ShouldNotExposeSetter()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.ImplementsGenericInterface(typeof(IQuery<>)));

		types.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in types)
			{
				foreach (var property in type.GetProperties())
				{
					property.GetSetMethod()
						.ShouldBeNull($"query '{type}' and property '{property}' should be immutable");
				}
			}
		});
	}

	[Fact]
	public void Application_Query_ShouldBeExcludedFromCodeCoverage()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.IsClass)
			.Where(a => a.ImplementsGenericInterface(typeof(IQuery<>)));

		types.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in types)
			{
				type.ShouldBeDecoratedWith<ExcludeFromCodeCoverageAttribute>();
			}
		});
	}

	[Fact]
	public void Application_Query_ShouldBeSealed()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.IsClass)
			.Where(a => a.ImplementsGenericInterface(typeof(IQuery<>)))
			.Where(a => a != typeof(PagedQuery<,>));

		types.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in types)
			{
				type.IsSealed
					.ShouldBeTrue($"query '{type}' should be sealed");
			}
		});
	}

	[Fact]
	public void Application_Query_ShouldHaveCorrespondingHandler()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies()
			.ToArray();
		var types = assemblies.GetAllTypes()
			.Where(a => a.IsClass)
			.Where(a => a.ImplementsGenericInterface(typeof(IQuery<>)))
			.Where(a => a != typeof(PagedQuery<,>));

		types.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in types)
			{
				assemblies.GetAllTypes()
					.SingleOrDefault(a => a.Name == $"{type.Name}Handler")
					.ShouldNotBeNull($"Query '{type}' should have corresponding QueryHandler");
			}
		});
	}
}
