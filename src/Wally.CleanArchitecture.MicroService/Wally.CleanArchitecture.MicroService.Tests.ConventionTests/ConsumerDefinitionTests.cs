using System.Linq;
using MassTransit;
using Shouldly;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Extensions;
using Xunit;

namespace Wally.CleanArchitecture.MicroService.Tests.ConventionTests;

public class ConsumerDefinitionTests
{
	[Fact]
	public void Infrastructure_AllClassesEndsWithConsumerDefinition_ShouldInheritConsumerDefinition()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes();
		var consumerDefinitionTypes = types.Where(a => a.Name.EndsWith("ConsumerDefinition"));

		consumerDefinitionTypes.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in consumerDefinitionTypes)
			{
				type.ShouldBeAssignableTo(typeof(ConsumerDefinition<>));
			}
		});
	}

	[Fact]
	public void Infrastructure_AllClassesInheritsConsumerDefinition_ShouldHaveConsumerDefinitionSuffix()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes();
		var consumerDefinitionTypes = types.Where(a => a.ImplementsGenericInterface(typeof(IConsumerDefinition<>)));

		consumerDefinitionTypes.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in consumerDefinitionTypes)
			{
				type.Name.ShouldEndWith("MessageConsumerDefinition");
			}
		});
	}

	[Fact]
	public void Infrastructure_AllClassesInheritsConsumerDefinition_ShouldHaveConsumerPrefix()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes();
		var consumerDefinitionTypes = types.Where(a => a.ImplementsGenericInterface(typeof(IConsumerDefinition<>)));

		consumerDefinitionTypes.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in consumerDefinitionTypes)
			{
				var genericType = type.BaseType!.GenericTypeArguments.Single();

				type.Name.ShouldBe(
					$"{genericType.Name}Definition",
					$"Type '{type}' should have name '{genericType.Name}Definition'");
			}
		});
	}
}
