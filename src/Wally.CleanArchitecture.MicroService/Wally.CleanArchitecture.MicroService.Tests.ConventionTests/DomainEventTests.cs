using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using Wally.CleanArchitecture.MicroService.Domain.Abstractions;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Extensions;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Helpers;
using Xunit;

namespace Wally.CleanArchitecture.MicroService.Tests.ConventionTests;

public class DomainEventTests
{
	[Fact]
	public void Domain_ClassesWhichInheritsDomainEvent_ShouldBeInDomainProject()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();

		using (new AssertionScope(new AssertionStrategy()))
		{
			foreach (var assembly in assemblies)
			{
				var types = assembly.Types()
					.ThatImplement<DomainEvent>();

				types.Should()
					.BeUnderNamespace($"{Configuration.Namespace}.Domain");
			}
		}
	}

	[Fact]
	public void Domain_AllClassesEndsWithDomainEvent_ShouldInheritDomainEvent()
	{
		var assemblies = Configuration.Assemblies.Domain;
		var types = assemblies.GetAllTypes();

		using (new AssertionScope())
		{
			foreach (var type in types.Where(a => a.Name.EndsWith("DomainEvent"))
						.Where(a => a != typeof(DomainEvent)))
			{
				type.Should()
					.BeAssignableTo<DomainEvent>();
			}
		}
	}

	[Fact]
	public void Domain_AllClassesWhichInheritsDomainEvent_ShouldHaveDomainEventSuffix()
	{
		var assemblies = Configuration.Assemblies.Domain;
		var types = assemblies.GetAllTypes();

		using (new AssertionScope(new AssertionStrategy()))
		{
			foreach (var type in types.ThatImplement<DomainEvent>())
			{
				type.Name.Should()
					.EndWith("DomainEvent");
			}
		}
	}

	[Fact]
	public void Domain_DomainEvent_ShouldNotExposeSetter()
	{
		var assemblies = Configuration.Assemblies.Domain;

		using (new AssertionScope(new AssertionStrategy()))
		{
			foreach (var assembly in assemblies)
			{
				var types = assembly.Types()
					.ThatImplement<DomainEvent>();

				types.Properties()
					.Should()
					.NotBeWritable("Request should be immutable");
			}
		}
	}

	[Fact]
	public void Domain_DomainEvent_ShouldBeExcludedFromCodeCoverage()
	{
		var assemblies = Configuration.Assemblies.Domain;

		using (new AssertionScope(new AssertionStrategy()))
		{
			foreach (var assembly in assemblies)
			{
				var types = assembly.Types()
					.ThatImplement<DomainEvent>();

				foreach (var type in types)
				{
					type.Should()
						.BeDecoratedWith<ExcludeFromCodeCoverageAttribute>();
				}
			}
		}
	}
}
