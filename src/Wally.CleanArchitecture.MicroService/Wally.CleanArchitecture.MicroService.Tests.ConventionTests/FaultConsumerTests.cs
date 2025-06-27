using System.Linq;
using MassTransit;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Extensions;

namespace Wally.CleanArchitecture.MicroService.Tests.ConventionTests;

public class FaultConsumerTests
{
	[Fact]
	public void Infrastructure_AllClassesEndsWithFaultConsumer_ShouldInheritConsumer()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.Name.EndsWith("FaultConsumer"));

		types.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in types)
			{
				type.ImplementsGenericInterface(typeof(IConsumer<>))
					.ShouldBeTrue();
			}
		});
	}

	[Fact]
	public void Infrastructure_AllClassesInheritsConsumerWithFaultParameter_ShouldHaveConsumerSuffix()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.ImplementsGenericInterface(typeof(IConsumer<>)))
			.Where(a => a.ImplementsGenericInterface(typeof(IConsumer<>)))
			.Where(a => a.GetGenericInterface(typeof(IConsumer<>)) !.GenericTypeArguments.Single()
				.IsGenericType)
			.Where(a => a.GetGenericInterface(typeof(IConsumer<>)) !.GenericTypeArguments.Single()
				.GetGenericTypeDefinition() == typeof(Fault<>));

		types.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in types)
			{
				type.Name.ShouldEndWith("MessageFaultConsumer");
			}
		});
	}

	[Fact]
	public void Infrastructure_AllClassesEndsWithFaultConsumer_ShouldHaveMessagePrefix()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.Name.EndsWith("FaultConsumer"))
			.Where(a => a.ImplementsGenericInterface(typeof(IConsumer<>)));

		types.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in types)
			{
				var genericType = type.GetGenericInterface(typeof(IConsumer<>)) !.GenericTypeArguments.Single();
				genericType = genericType.GenericTypeArguments.Single();

				type.Name.ShouldBe(
						$"{genericType.Name}FaultConsumer",
						$"Fault Consumer '{type}' name should be '{genericType.Name}FaultConsumer'");
			}
		});
	}
}
