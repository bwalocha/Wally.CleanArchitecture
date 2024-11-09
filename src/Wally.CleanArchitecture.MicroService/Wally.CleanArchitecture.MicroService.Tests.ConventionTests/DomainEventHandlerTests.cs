using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using Wally.CleanArchitecture.MicroService.Application.Abstractions;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Extensions;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Helpers;
using Xunit;

namespace Wally.CleanArchitecture.MicroService.Tests.ConventionTests;

public class DomainEventHandlerTests
{
	[Fact]
	public void Domain_ClassesWhichInheritsDomainEventHandler_ShouldBeInApplicationProject()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.ImplementsGenericInterface(typeof(IDomainEventHandler<>)));

		using (new AssertionScope(new AssertionStrategy()))
		{
			types.Types()
				.Should()
				.BeUnderNamespace($"{Configuration.Namespace}.Application");
		}
	}

	[Fact]
	public void Domain_AllClassesEndsWithDomainEventHandler_ShouldInheritDomainEventHandler()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes();

		using (new AssertionScope())
		{
			foreach (var type in types.Where(a => a.Name.EndsWith("DomainEventHandler")))
			{
				type.Should()
					.BeAssignableTo(typeof(IDomainEventHandler<>));
			}
		}
	}

	[Fact]
	public void Domain_AllClassesWhichInheritsDomainEventHandler_ShouldHaveDomainEventHandlerSuffix()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes();

		using (new AssertionScope(new AssertionStrategy()))
		{
			foreach (var type in types
						.Where(a => a.IsClass)
						.Where(a => a.ImplementsGenericInterface(typeof(IDomainEventHandler<>))))
			{
				type.Name.Should()
					.EndWith("DomainEventHandler");
			}
		}
	}
}
