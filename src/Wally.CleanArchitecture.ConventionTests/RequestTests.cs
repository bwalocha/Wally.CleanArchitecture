using System.Linq;
using System.Text.RegularExpressions;

using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Types;

using Wally.CleanArchitecture.Application.Users.Commands;
using Wally.CleanArchitecture.Contracts.Requests.User;
using Wally.CleanArchitecture.ConventionTests.Helpers;
using Wally.Lib.DDD.Abstractions.Requests;

using Xunit;

namespace Wally.CleanArchitecture.ConventionTests;

public class RequestTests
{
	[Fact]
	public void Application_Request_ShouldNotExposeSetter()
	{
		var applicationTypes = AllTypes.From(typeof(UpdateUserCommandHandler).Assembly);

		applicationTypes.ThatImplement<IRequest>()
			.Properties()
			.Should()
			.NotBeWritable("request should be immutable");
	}

	[Fact]
	public void Application_ClassesWhichImplementsIRequest_ShouldBeInTheSameProject()
	{
		var applicationNamespace = Regex.Match(
				typeof(GetUsersRequest).Namespace!,
				@"Wally.CleanArchitecture\.Contracts\.Requests(?=[\.$])")
			.Value;

		using (new AssertionScope(new AssertionStrategy()))
		{
			foreach (var assembly in TypeHelpers.GetAllInternalAssemblies())
			{
				var types = AllTypes.From(assembly);

				types.ThatImplement<IRequest>()
					.Should()
					.BeUnderNamespace(applicationNamespace);
			}
		}
	}

	[Fact]
	public void Application_AllClassessEndsWithRequest_ShouldImplementIRequest()
	{
		var applicationTypes = AllTypes.From(typeof(UpdateUserCommandHandler).Assembly);

		using (new AssertionScope())
		{
			foreach (var type in applicationTypes.Where(x => x.Name.EndsWith("Request") && x.Name != nameof(IRequest)))
			{
				type.Should()
					.Implement<IRequest>();
			}
		}
	}

	[Fact]
	public void Application_AllClassessImplementsIRequest_ShouldHaveRequestSuffix()
	{
		var applicationTypes = AllTypes.From(typeof(IRequest).Assembly);

		using (new AssertionScope())
		{
			foreach (var type in applicationTypes.ThatImplement<IRequest>())
			{
				type.Name.Should()
					.EndWith("Request");
			}
		}
	}

	[Fact]
	public void Application_AllClassessImplementsIRequest_ShouldHaveCorrespondingValidator()
	{
		var assemblies = TypeHelpers.GetAllInternalAssemblies();

		using (new AssertionScope(new AssertionStrategy()))
		{
			foreach (var assembly in assemblies)
			{
				foreach (var type in assembly.GetTypes()
							.ThatImplement<IRequest>())
				{
					assemblies.SelectMany(a => a.GetTypes())
						.SingleOrDefault(a => a.Name == $"{type.Name}Validator")
						.Should()
						.NotBeNull("every Request '{0}' should have corresponding Validator", type);
				}
			}
		}
	}
}
