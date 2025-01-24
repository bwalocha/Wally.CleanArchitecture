using System.Linq;
using MassTransit;
using Shouldly;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Extensions;
using Xunit;

namespace Wally.CleanArchitecture.MicroService.Tests.ConventionTests;

public class ConsumerTests
{
	[Fact]
	public void Infrastructure_AllClassesEndsWithConsumer_ShouldInheritConsumer()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.Name.EndsWith("Consumer"));

		types.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in types)
			{
				type.ImplementsGenericInterface(typeof(IConsumer<>))
					.ShouldBeTrue($"Type '{type}' should implement '{typeof(IConsumer<>).Name}'");
			}
		});
	}

	[Fact]
	public void Infrastructure_AllClassesInheritsConsumer_ShouldHaveConsumerSuffix()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => !a.Name.EndsWith("FaultConsumer"))
			.Where(a => a.ImplementsGenericInterface(typeof(IConsumer<>)));

		types.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in types)
			{
				type.Name.ShouldEndWith("MessageConsumer");
			}
		});
	}

	[Fact]
	public void Infrastructure_AllClassesInheritsConsumer_ShouldHaveMessagePrefix()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => !a.Name.EndsWith("FaultConsumer"))
			.Where(a => a.ImplementsGenericInterface(typeof(IConsumer<>)));

		types.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in types)
			{
				var genericType = type.GetGenericInterface(typeof(IConsumer<>)) !.GenericTypeArguments.Single();

				type.Name.ShouldBe(
					$"{genericType.Name}Consumer",
					$"Consumer '{type}' name should be '{genericType.Name}Consumer'");
			}
		});
	}

	[Fact]
	public void Infrastructure_AllClassesInheritsConsumer_ShouldHaveFaultConsumer()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes().ToArray();
		var customerTypes = types.Where(a => !a.Name.EndsWith("FaultConsumer"))
			.Where(a => a.ImplementsGenericInterface(typeof(IConsumer<>)));

		customerTypes.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in customerTypes)
			{
				var genericType = type.GetGenericInterface(typeof(IConsumer<>)) !.GenericTypeArguments.Single();

				types.SingleOrDefault(a => a.Name == $"{genericType.Name}FaultConsumer")
					.ShouldNotBeNull(
						$"Consumer '{type}' should have corresponding Fault Consumer '{genericType.Name}FaultConsumer'");
			}
		});
	}

	[Fact]
	public void Infrastructure_AllClassesInheritsConsumer_ShouldHaveConsumerDefinition()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes().ToArray();
		var customerTypes = types.Where(a => !a.Name.EndsWith("FaultConsumer"))
			.Where(a => a.ImplementsGenericInterface(typeof(IConsumer<>)));

		customerTypes.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in customerTypes)
			{
				var genericType = type.GetGenericInterface(typeof(IConsumer<>)) !.GenericTypeArguments.Single();

				types.SingleOrDefault(a => a.Name == $"{genericType.Name}ConsumerDefinition")
					.ShouldNotBeNull($"Consumer '{type}' should have corresponding Consumer Definition '{genericType.Name}ConsumerDefinition'");
			}
		});
	}
}
