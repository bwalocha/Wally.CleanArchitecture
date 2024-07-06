using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Extensions;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Helpers;
using Wally.Lib.DDD.Abstractions.Commands;
using Xunit;

namespace Wally.CleanArchitecture.MicroService.Tests.ConventionTests;

public class CommandHandlerTests
{
	[Fact]
	public void Application_AllClassesEndsWithCommandHandler_ShouldImplementICommandHandler()
	{
		var applicationTypes = Configuration.Assemblies.Application.GetAllTypes();

		using (new AssertionScope(new AssertionStrategy()))
		{
			foreach (var type in applicationTypes.Where(a => a.Name.EndsWith("CommandHandler")))
			{
				type.Should()
					.BeAssignableTo(
						type.ImplementsGenericInterface(typeof(ICommandHandler<>))
							? typeof(ICommandHandler<>)
							: typeof(ICommandHandler<,>),
						"All command handlers should implement ICommandHandler interface");
			}
		}
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

		using (new AssertionScope(new AssertionStrategy()))
		{
			foreach (var type in commandHandlerTypes)
			{
				type.Name
					.Should()
					.EndWith("CommandHandler", "All command handlers name should ends with 'CommanHandler'");
			}
		}
	}
}
