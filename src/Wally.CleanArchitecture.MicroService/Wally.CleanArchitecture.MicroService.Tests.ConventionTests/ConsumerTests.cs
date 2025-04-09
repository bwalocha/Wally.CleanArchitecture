using System;
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
		// Assign
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.Name.EndsWith("Consumer"));

		// Act
		void Act(Type type) =>
			type.ImplementsGenericInterface(typeof(IConsumer<>))
				.ShouldBeTrue($"Type '{type}' should implement '{typeof(IConsumer<>).Name}'");

		// Assert
		types.ShouldSatisfyAllConditions(types.Select(type => (Action)(() => Act(type)))
			.ToArray());
	}

	[Fact]
	public void Infrastructure_AllClassesInheritsConsumer_ShouldHaveConsumerSuffix()
	{
		// Assign
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => !a.Name.EndsWith("FaultConsumer"))
			.Where(a => a.ImplementsGenericInterface(typeof(IConsumer<>)));

		// Act
		void Act(Type type) =>
			type.Name.ShouldEndWith("MessageConsumer");
		
		// Assert
		types.ShouldSatisfyAllConditions(types.Select(type => (Action)(() => Act(type)))
			.ToArray());
	}

	[Fact]
	public void Infrastructure_AllClassesInheritsConsumer_ShouldHaveMessagePrefix()
	{
		// Assign
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => !a.Name.EndsWith("FaultConsumer"))
			.Where(a => a.ImplementsGenericInterface(typeof(IConsumer<>)));

		// Act
		void Act(Type type)
		{
			var genericType = type.GetGenericInterface(typeof(IConsumer<>)) !.GenericTypeArguments.Single();

			type.Name.ShouldBe($"{genericType.Name}Consumer", $"Consumer '{type}' name should be '{genericType.Name}Consumer'");
		}

		// Assert
		types.ShouldSatisfyAllConditions(types.Select(type => (Action)(() => Act(type)))
			.ToArray());
	}

	[Fact]
	public void Infrastructure_AllClassesInheritsConsumer_ShouldHaveFaultConsumer()
	{
		// Assign
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes().ToArray();
		var customerTypes = types.Where(a => !a.Name.EndsWith("FaultConsumer"))
			.Where(a => a.ImplementsGenericInterface(typeof(IConsumer<>)));

		// Act
		void Act(Type type)
		{
			var genericType = type.GetGenericInterface(typeof(IConsumer<>)) !.GenericTypeArguments.Single();

			types.SingleOrDefault(a => a.Name == $"{genericType.Name}FaultConsumer")
				.ShouldNotBeNull(
					$"Consumer '{type}' should have corresponding Fault Consumer '{genericType.Name}FaultConsumer'");
		}

		// Assert
		types.ShouldSatisfyAllConditions(customerTypes.Select(type => (Action)(() => Act(type)))
			.ToArray());
	}

	[Fact]
	public void Infrastructure_AllClassesInheritsConsumer_ShouldHaveConsumerDefinition()
	{
		// Assign
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes().ToArray();
		var customerTypes = types.Where(a => !a.Name.EndsWith("FaultConsumer"))
			.Where(a => a.ImplementsGenericInterface(typeof(IConsumer<>)));

		// Act
		void Act(Type type)
		{
			var genericType = type.GetGenericInterface(typeof(IConsumer<>)) !.GenericTypeArguments.Single();

			types.SingleOrDefault(a => a.Name == $"{genericType.Name}ConsumerDefinition")
				.ShouldNotBeNull($"Consumer '{type}' should have corresponding Consumer Definition '{genericType.Name}ConsumerDefinition'");
		}

		// Assert
		types.ShouldSatisfyAllConditions(customerTypes.Select(type => (Action)(() => Act(type)))
			.ToArray());
	}
}
