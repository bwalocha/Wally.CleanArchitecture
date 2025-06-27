using System.Linq;
using MassTransit;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Extensions;

namespace Wally.CleanArchitecture.MicroService.Tests.ConventionTests;

public class ConsumerDefinitionTests
{
	[Fact]
	public void Infrastructure_AllClassesEndsWithConsumerDefinition_ShouldInheritConsumerDefinition()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.Name.EndsWith("ConsumerDefinition"))
			.Select(a => a.DeclaringType);

		types.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in types)
			{
				type.ShouldBeAssignableTo(typeof(ConsumerDefinition<>));
			}
		});
	}

	[Fact]
	public void Infrastructure_AllClassesInheritsConsumerDefinition_ShouldHaveConsumerDefinitionSuffix()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.ImplementsGenericInterface(typeof(IConsumerDefinition<>)));

		types.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in types)
			{
				type.Name.ShouldEndWith("MessageConsumerDefinition");
			}
		});
	}

	[Fact]
	public void Infrastructure_AllClassesInheritsConsumerDefinition_ShouldHaveConsumerPrefix()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.ImplementsGenericInterface(typeof(IConsumerDefinition<>)));

		types.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in types)
			{
				var genericType = type.BaseType!.GenericTypeArguments.Single();

				type.Name.ShouldBe(
					$"{genericType.Name}Definition",
					$"Type '{type}' name should be '{genericType.Name}Definition'");
			}
		});
	}
	
	[Fact]
	public void Infrastructure_AllClassesInheritsConsumerDefinition_ShouldHaveConsumer()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes().ToArray();
		var customerTypes = types
			.Where(a => a.ImplementsGenericInterface(typeof(IConsumerDefinition<>)));

		customerTypes.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in customerTypes)
			{
				var genericType = type.GetGenericInterface(typeof(IConsumerDefinition<>)) !.GenericTypeArguments.Single();

				types.SingleOrDefault(a => a.Name == genericType.Name)
					.ShouldNotBeNull(
						$"Consumer Definition '{type}' should have corresponding Consumer '{genericType.Name}'");
			}
		});
	}
}
