using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using MassTransit;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Extensions;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Helpers;
using Xunit;

namespace Wally.CleanArchitecture.MicroService.Tests.ConventionTests;

public class ConsumerTests
{
	[Fact]
	public void Infrastructure_AllClassesEndsWithConsumer_ShouldInheritConsumer()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes();

		using var scope = new AssertionScope(new AssertionStrategy());
		foreach (var type in types.Where(a => a.Name.EndsWith("Consumer")))
		{
			type.Should()
				.BeAssignableTo(typeof(IConsumer<>));
		}
	}

	[Fact]
	public void Infrastructure_AllClassesInheritsConsumer_ShouldHaveConsumerSuffix()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes();

		using var scope = new AssertionScope(new AssertionStrategy());
		foreach (var type in types.Where(a => !a.Name.EndsWith("FaultConsumer"))
					.Where(a => a.ImplementsGenericInterface(typeof(IConsumer<>))))
		{
			type.Name.Should()
				.EndWith("MessageConsumer");
		}
	}

	[Fact]
	public void Infrastructure_AllClassesInheritsConsumer_ShouldHaveMessagePrefix()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes();

		using var scope = new AssertionScope(new AssertionStrategy());
		foreach (var type in types.Where(a => !a.Name.EndsWith("FaultConsumer"))
					.Where(a => a.ImplementsGenericInterface(typeof(IConsumer<>))))
		{
			var genericType = type.GetGenericInterface(typeof(IConsumer<>)) !.GenericTypeArguments.Single();

			type.Name.Should()
				.Be(
					$"{genericType.Name}Consumer",
					"Type '{0}' should have name '{1}'",
					type,
					$"{genericType.Name}Consumer");
		}
	}

	[Fact]
	public void Infrastructure_AllClassesEndsWithConsumer_ShouldHaveFaultConsumer()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes();

		using var scope = new AssertionScope(new AssertionStrategy());
		foreach (var type in types.Where(a => !a.Name.EndsWith("FaultConsumer"))
					.Where(a => a.ImplementsGenericInterface(typeof(IConsumer<>))))
		{
			var genericType = type.GetGenericInterface(typeof(IConsumer<>)) !.GenericTypeArguments.Single();

			types.SingleOrDefault(a => a.Name == $"{genericType.Name}FaultConsumer")
				.Should()
				.NotBeNull("Type '{0}' should have corresponding FaultConsumer '{1}'",
					type,
					$"{genericType.Name}FaultConsumer");
		}
	}

	[Fact]
	public void Infrastructure_AllClassesEndsWithConsumer_ShouldHaveConsumerDefinition()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes();

		using var scope = new AssertionScope(new AssertionStrategy());
		foreach (var type in types.Where(a => !a.Name.EndsWith("FaultConsumer"))
					.Where(a => a.ImplementsGenericInterface(typeof(IConsumer<>))))
		{
			var genericType = type.GetGenericInterface(typeof(IConsumer<>)) !.GenericTypeArguments.Single();

			types.SingleOrDefault(a => a.Name == $"{genericType.Name}ConsumerDefinition")
				.Should()
				.NotBeNull("Type '{0}' should have corresponding ConsumerDefinition '{1}'",
					type,
					$"{genericType.Name}ConsumerDefinition");
		}
	}
}
