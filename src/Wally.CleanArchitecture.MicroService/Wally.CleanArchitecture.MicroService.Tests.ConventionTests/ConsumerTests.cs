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
		var types = assemblies.GetAllTypes();
		var customerTypes = types.Where(a => a.Name.EndsWith("Consumer"));

		customerTypes.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in customerTypes)
			{
				type.ShouldBeAssignableTo(typeof(IConsumer<>));
			}
		});
	}

	[Fact]
	public void Infrastructure_AllClassesInheritsConsumer_ShouldHaveConsumerSuffix()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes();
		var customerTypes = types.Where(a => !a.Name.EndsWith("FaultConsumer"))
			.Where(a => a.ImplementsGenericInterface(typeof(IConsumer<>)));

		customerTypes.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in customerTypes)
			{
				type.Name.ShouldEndWith("MessageConsumer");
			}
		});
	}

	[Fact]
	public void Infrastructure_AllClassesInheritsConsumer_ShouldHaveMessagePrefix()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes();
		var customerTypes = types.Where(a => !a.Name.EndsWith("FaultConsumer"))
			.Where(a => a.ImplementsGenericInterface(typeof(IConsumer<>)));

		customerTypes.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in customerTypes)
			{
				var genericType = type.GetGenericInterface(typeof(IConsumer<>)) !.GenericTypeArguments.Single();

				type.Name.ShouldBe(
					$"{genericType.Name}Consumer",
					$"Type '{type}' should have name '{genericType.Name}Consumer'");
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
						$"Type '{type}' should have corresponding FaultConsumer '{genericType.Name}FaultConsumer'");
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
					.ShouldNotBeNull($"Type '{type}' should have corresponding ConsumerDefinition '{genericType.Name}ConsumerDefinition'");
			}
		});
	}
}
