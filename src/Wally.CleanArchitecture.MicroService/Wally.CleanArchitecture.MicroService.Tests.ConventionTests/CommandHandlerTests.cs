using System.Linq;
using Shouldly;
using Wally.CleanArchitecture.MicroService.Application.Abstractions;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Extensions;
using Xunit;

namespace Wally.CleanArchitecture.MicroService.Tests.ConventionTests;

public class CommandHandlerTests
{
	[Fact]
	public void Application_AllClassesEndsWithCommandHandler_ShouldImplementICommandHandler()
	{
		var applicationTypes = Configuration.Assemblies.Application.GetAllTypes();
		var commandHandlerTypes = applicationTypes.Where(a => a.Name.EndsWith("CommandHandler"));

		commandHandlerTypes.ShouldSatisfyAllConditions(
			() =>
			{
				foreach (var type in commandHandlerTypes)
				{
					type.ShouldBeAssignableTo(
						type.ImplementsGenericInterface(typeof(ICommandHandler<>))
							? typeof(ICommandHandler<>)
							: typeof(ICommandHandler<,>),
						"All command handlers should implement ICommandHandler interface");
				}
			});
	}

	[Fact]
	public void Application_AllClassesImplementedICommandHandler_ShouldEndsWithCommandHandler()
	{
		var applicationTypes = Configuration.Assemblies.Application.GetAllTypes();
		var commandHandlerTypes = applicationTypes
			.Where(a => a.IsClass)
			.Where(a => !a.IsAbstract)
			.Where(
				a => a.ImplementsGenericInterface(typeof(ICommandHandler<>)) ||
					a.ImplementsGenericInterface(typeof(ICommandHandler<,>)));

		commandHandlerTypes.ShouldSatisfyAllConditions(
			() =>
			{
				foreach (var type in commandHandlerTypes)
				{
					type.Name
						.ShouldEndWith("CommandHandler", Case.Sensitive,
							"All CommandHandler names should end with 'CommandHandler'");
				}
			});
	}
}
