using System.Linq;

using FluentAssertions;
using FluentAssertions.Execution;

using MassTransit;

using Wally.CleanArchitecture.MicroService.ConventionTests.Helpers;

using Xunit;

namespace Wally.CleanArchitecture.MicroService.ConventionTests;

public class ConsumerTests
{
	[Fact]
	public void Infrastructure_AllClassesEndsWithConsumer_ShouldInheritConsumer()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes();

		using (new AssertionScope())
		{
			foreach (var type in types.Where(a => a.Name.EndsWith("Consumer")))
			{
				type.Should()
					.BeAssignableTo(typeof(IConsumer<>));
			}
		}
	}

	[Fact]
	public void Application_AllClassesInheritsConsumer_ShouldHaveConsumerSuffix()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes();

		using (new AssertionScope())
		{
			foreach (var type in types.Where(a => a.Name.EndsWith("Consumer"))
						.Where(a => a.InheritsGenericClass(typeof(IConsumer<>))))
			{
				type.Name.Should()
					.EndWith("MessageConsumer");
			}
		}
	}

	[Fact]
	public void Application_AllClassesInheritsConsumer_ShouldHaveMessagePrefix()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes();

		using (new AssertionScope())
		{
			foreach (var type in types.Where(a => a.Name.EndsWith("Consumer"))
						.Where(a => a.InheritsGenericClass(typeof(IConsumer<>))))
			{
				var genericType = type.BaseType!.GenericTypeArguments.Single();

				type.Name.Should()
					.Be(
						$"{genericType.Name}Consumer",
						"Type '{0}' should have name '{1}'",
						type,
						$"{genericType.Name}Consumer");
			}
		}
	}
}