using System.Linq;
using Wally.CleanArchitecture.MicroService.Application.Abstractions;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Extensions;

namespace Wally.CleanArchitecture.MicroService.Tests.ConventionTests;

public class CommandHandlerTests
{
	[Fact]
	public void Application_AllClassesEndsWithCommandHandler_ShouldImplementICommandHandler()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.Name.EndsWith("CommandHandler"));

		types.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in types)
			{
				if (type.ImplementsGenericInterface(typeof(ICommandHandler<>)))
				{
					continue;
				}

				type.ImplementsGenericInterface(typeof(ICommandHandler<,>))
					.ShouldBeTrue($"Command Handler '{type}' should implement 'ICommandHandler' interface");
			}
		});
	}

	[Fact]
	public void Application_AllClassesImplementedICommandHandler_ShouldHaveNameSuffix()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.IsClass)
			.Where(a => !a.IsAbstract)
			.Where(
				a => a.ImplementsGenericInterface(typeof(ICommandHandler<>)) ||
					a.ImplementsGenericInterface(typeof(ICommandHandler<,>)));

		types.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in types)
			{
				type.Name
					.ShouldEndWith("CommandHandler", Case.Sensitive,
						$"Command Handler '{type}' name should end with 'CommandHandler'");
			}
		});
	}
}
