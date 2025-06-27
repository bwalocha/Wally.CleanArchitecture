using System.Linq;
using Wally.CleanArchitecture.MicroService.Application.Abstractions;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Extensions;

namespace Wally.CleanArchitecture.MicroService.Tests.ConventionTests;

public class DomainEventHandlerTests
{
	[Fact]
	public void Domain_AllClassesEndsWithDomainEventHandler_ShouldInheritDomainEventHandler()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.Name.EndsWith("DomainEventHandler"));

		types.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in types)
			{
				type.ImplementsGenericInterface(typeof(IDomainEventHandler<>))
					.ShouldBeTrue($"Domain Event Handler '{type}' should implement 'IDomainEventHandler' interface");
			}
		});
	}
	
	[Fact]
	public void Domain_AllClassesWhichInheritsDomainEventHandler_ShouldHaveNameSuffix()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.IsClass)
			.Where(a => a.ImplementsGenericInterface(typeof(IDomainEventHandler<>)));

		types.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in types)
			{
				type.Name.ShouldEndWith("DomainEventHandler", Case.Sensitive,
					$"Domain Event Handler '{type}' name should end with 'DomainEventHandler'");
			}
		});
	}

	[Fact]
	public void Domain_ClassesWhichInheritsDomainEventHandler_ShouldBeInApplicationProject()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.ImplementsGenericInterface(typeof(IDomainEventHandler<>)));

		types.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in types)
			{
				type.Namespace.ShouldStartWith($"{Configuration.Namespace}.Application");
			}
		});
	}
}
