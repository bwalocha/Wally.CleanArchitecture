using System.Linq;

using FluentAssertions;
using FluentAssertions.Execution;

using Wally.CleanArchitecture.MicroService.ConventionTests.Helpers;
using Wally.Lib.DDD.Abstractions.DomainEvents;

using Xunit;

namespace Wally.CleanArchitecture.MicroService.ConventionTests;

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
					.BeUnderNamespace("Wally.CleanArchitecture.MicroService.Domain");
			}
		}
	}

	[Fact]
	public void Domain_AllClassesEndsWithEvent_ShouldInheritDomainEvent()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes();

		using (new AssertionScope())
		{
			foreach (var type in types.Where(a => a.Name.EndsWith("Event"))
						.Where(a => a != typeof(DomainEvent)))
			{
				type.Should()
					.BeAssignableTo<DomainEvent>();
			}
		}
	}

	[Fact]
	public void Domain_AllClassesWhichInheritsDomainEvent_ShouldHasEventSuffix()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes();

		using (new AssertionScope(new AssertionStrategy()))
		{
			foreach (var type in types.ThatImplement<DomainEvent>())
			{
				type.Name.Should()
					.EndWith("Event");
			}
		}
	}

	[Fact]
	public void Domain_DomainEvent_ShouldNotExposeSetter()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();

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
}
