using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using MassTransit;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Extensions;
using Xunit;

namespace Wally.CleanArchitecture.MicroService.Tests.ConventionTests;

public class FaultConsumerTests
{
	[Fact]
	public void Infrastructure_AllClassesEndsWithFaultConsumer_ShouldInheritConsumer()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes();
		
		using (new AssertionScope())
		{
			foreach (var type in types.Where(a => a.Name.EndsWith("FaultConsumer")))
			{
				type.Should()
					.BeAssignableTo(typeof(IConsumer<>));
			}
		}
	}
	
	[Fact]
	public void Infrastructure_AllClassesInheritsConsumerWithFaultParameter_ShouldHaveConsumerSuffix()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes();
		
		using (new AssertionScope())
		{
			foreach (var type in types.Where(a => a.ImplementsGenericInterface(typeof(IConsumer<>)))
						.Where(a => a.ImplementsGenericInterface(typeof(IConsumer<>)))
						.Where(a => a.GetGenericInterface(typeof(IConsumer<>)) !.GenericTypeArguments.Single()
							.IsGenericType)
						.Where(a => a.GetGenericInterface(typeof(IConsumer<>)) !.GenericTypeArguments.Single()
							.GetGenericTypeDefinition() == typeof(Fault<>)))
			{
				type.Name.Should()
					.EndWith("MessageFaultConsumer");
			}
		}
	}
	
	[Fact]
	public void Infrastructure_AllClassesEndsWithFaultConsumer_ShouldHaveMessagePrefix()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes();
		
		using (new AssertionScope())
		{
			foreach (var type in types.Where(a => a.Name.EndsWith("FaultConsumer"))
						.Where(a => a.ImplementsGenericInterface(typeof(IConsumer<>))))
			{
				var genericType = type.GetGenericInterface(typeof(IConsumer<>)) !.GenericTypeArguments.Single();
				genericType = genericType.GenericTypeArguments.Single();
				
				type.Name.Should()
					.Be(
						$"{genericType.Name}FaultConsumer",
						"Type '{0}' should have name '{1}'",
						type,
						$"{genericType.Name}FaultConsumer");
			}
		}
	}
}
