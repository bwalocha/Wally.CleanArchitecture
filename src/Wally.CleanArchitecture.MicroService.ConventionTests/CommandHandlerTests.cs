using System.Linq;

using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Types;

using Wally.CleanArchitecture.MicroService.Application.Users.Commands;
using Wally.CleanArchitecture.MicroService.ConventionTests.Helpers;
using Wally.Lib.DDD.Abstractions.Commands;

using Xunit;

namespace Wally.CleanArchitecture.MicroService.ConventionTests;

public class CommandHandlerTests
{
	[Fact]
	public void Application_AllClassesEndsWithCommandHandler_ShouldImplementICommandHandler()
	{
		var applicationTypes = AllTypes.From(typeof(UpdateUserCommand).Assembly);

		using (new AssertionScope(new AssertionStrategy()))
		{
			foreach (var type in applicationTypes.Where(a => a.Name.EndsWith("CommandHandler")))
			{
				type.Should()
					.BeAssignableTo(
						typeof(ICommandHandler<>),
						"All command handlers should implement ICommandHandler interface");

				// TODO: add tests for ICommandHandler<,>
				/*type.Should()
					.BeAssignableTo(
						typeof(ICommandHandler<,>),
						"All command handlers should implement ICommandHandler interface");*/
			}
		}
	}
}
