using System.Linq;

using FluentAssertions;
using FluentAssertions.Execution;

using MassTransit;

using Wally.CleanArchitecture.MicroService.ConventionTests.Helpers;

using Xunit;

namespace Wally.CleanArchitecture.MicroService.ConventionTests;

public class ConsumerDefinitionTests
{
	[Fact]
	public void Infrastructure_AllClassesEndsWithConsumerDefinition_ShouldInheritConsumerDefinition()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes();

		using (new AssertionScope())
		{
			foreach (var type in types.Where(a => a.Name.EndsWith("ConsumerDefinition")))
			{
				type.Should()
					.BeAssignableTo(typeof(ConsumerDefinition<>));
			}
		}
	}

	[Fact]
	public void Infrastructure_AllClassesInheritsConsumerDefinition_ShouldHaveConsumerDefinitionSuffix()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes();

		using (new AssertionScope())
		{
			foreach (var type in types.Where(a => a.Name.EndsWith("ConsumerDefinition"))
						.Where(a => a.ImplementsGenericInterface(typeof(IConsumerDefinition<>))))
			{
				type.Name.Should()
					.EndWith("MessageConsumerDefinition");
			}
		}
	}

	[Fact]
	public void Infrastructure_AllClassesInheritsConsumerDefinition_ShouldHaveConsumerPrefix()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes();

		using (new AssertionScope())
		{
			foreach (var type in types.Where(a => a.Name.EndsWith("ConsumerDefinition"))
						.Where(a => a.ImplementsGenericInterface(typeof(IConsumerDefinition<>))))
			{
				var genericType = type.BaseType!.GenericTypeArguments.Single();

				type.Name.Should()
					.Be(
						$"{genericType.Name}Definition",
						"Type '{0}' should have name '{1}'",
						type,
						$"{genericType.Name}Definition");
			}
		}
	}
}
