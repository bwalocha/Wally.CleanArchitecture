using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Types;
using Wally.CleanArchitecture.Application.Users.Commands;
using Wally.CleanArchitecture.ConventionTests.Helpers;
using Wally.Lib.DDD.Abstractions.Commands;
using Xunit;

namespace Wally.CleanArchitecture.ConventionTests;

public class CommandHandlerTests
{
	[Fact]
	public void Application_AllClassesEndsWithCommandHandler_ShouldImplementICommandHandler()
	{
		var applicationTypes = AllTypes.From(typeof(UpdateUserCommand).Assembly);

		using (new AssertionScope(new AssertionStrategy()))
		{
			foreach (var type in applicationTypes.Where(x => x.Name.EndsWith("CommandHandler")))
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
