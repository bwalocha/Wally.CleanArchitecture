using System.Linq;
using System.Text.RegularExpressions;

using FluentAssertions;
using FluentAssertions.Common;
using FluentAssertions.Execution;
using FluentAssertions.Types;

using Wally.CleanArchitecture.MicroService.Contracts.Requests.Users;
using Wally.CleanArchitecture.MicroService.ConventionTests.Helpers;
using Wally.Lib.DDD.Abstractions.Requests;

using Xunit;

namespace Wally.CleanArchitecture.MicroService.ConventionTests;

public class RequestTests
{
	[Fact]
	public void Application_Request_ShouldNotExposeSetter()
	{
		using (new AssertionScope(new AssertionStrategy()))
		{
			foreach (var assembly in TypeHelpers.GetAllInternalAssemblies())
			{
				var types = AllTypes.From(assembly)
					.ThatImplement<IRequest>();

				foreach (var type in types)
				{
					foreach (var property in type.Properties())
					{
						if (property.SetMethod?.IsPublic != true)
						{
							continue;
						}

						property.Should()
							.BeWritable(
								CSharpAccessModifier.Private,
								"Response class '{0}' should not expose setter '{1}'",
								type,
								property);
					}
				}
			}
		}
	}

	[Fact]
	public void Application_ClassesWhichImplementsIRequest_ShouldBeInTheSameProject()
	{
		var applicationNamespace = Regex.Match(
				typeof(GetUsersRequest).Namespace!,
				@"Wally.CleanArchitecture.MicroService\.Contracts\.Requests(?=[\.$])")
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
		var applicationTypes = AllTypes.From(typeof(UpdateUserRequest).Assembly);

		using (new AssertionScope())
		{
			foreach (var type in applicationTypes.Where(a => a.Name.EndsWith("Request") && a.Name != nameof(IRequest)))
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
