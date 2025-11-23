using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Wally.CleanArchitecture.MicroService.Domain.Abstractions;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Extensions;

namespace Wally.CleanArchitecture.MicroService.Tests.ConventionTests;

public class DomainEventTests
{
	[Fact]
	public void Domain_ClassesWhichInheritsDomainEvent_ShouldBeInDomainProject()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.InheritsClass(typeof(DomainEvent)));

		types.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in types)
			{
				type.Namespace.ShouldStartWith($"{Configuration.Namespace}.Domain");
			}
		});
	}

	[Fact]
	public void Domain_AllClassesEndsWithDomainEvent_ShouldInheritDomainEvent()
	{
		var assemblies = Configuration.Assemblies.Domain;
		var types = assemblies.GetAllTypes()
			.Where(a => a.Name.EndsWith("DomainEvent"))
			.Where(a => a != typeof(DomainEvent));

		types.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in types)
			{
				type.InheritsClass(typeof(DomainEvent))
					.ShouldBeTrue();
			}
		});
	}

	[Fact]
	public void Domain_AllClassesWhichInheritsDomainEvent_ShouldHaveDomainEventSuffix()
	{
		var assemblies = Configuration.Assemblies.Domain;
		var types = assemblies.GetAllTypes()
			.Where(a => a.InheritsClass(typeof(DomainEvent)));

		types.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in types)
			{
				type.Name.ShouldEndWith("DomainEvent");
			}
		});
	}

	[Fact]
	public void Domain_DomainEvent_ShouldNotExposeSetter()
	{
		var assemblies = Configuration.Assemblies.Domain;
		var types = assemblies.GetAllTypes()
			.Where(a => a.InheritsClass(typeof(DomainEvent)));

		types.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in types)
			{
				foreach (var property in type.GetProperties())
				{
					property.GetSetMethod()
						.ShouldBeNull($"Request '{type}' and property '{property}' should be immutable");
				}
			}
		});
	}

	[Fact]
	public void Domain_DomainEvent_ShouldBeExcludedFromCodeCoverage()
	{
		var assemblies = Configuration.Assemblies.Domain;
		var types = assemblies.GetAllTypes()
			.Where(a => a.InheritsClass(typeof(DomainEvent)));

		types.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in types)
			{
				type.ShouldBeDecoratedWith<ExcludeFromCodeCoverageAttribute>(
					$"Domain Event '{type}' is not excluded from code coverage");
			}
		});
	}
}
