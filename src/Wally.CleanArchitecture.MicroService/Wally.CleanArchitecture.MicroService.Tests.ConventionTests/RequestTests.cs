using System.Linq;
using FluentAssertions;
using FluentAssertions.Common;
using FluentAssertions.Execution;
using FluentAssertions.Types;
using Wally.CleanArchitecture.MicroService.Application.Contracts;
using Wally.CleanArchitecture.MicroService.Application.Contracts.Abstractions;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Extensions;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Helpers;
using Xunit;

namespace Wally.CleanArchitecture.MicroService.Tests.ConventionTests;

public class RequestTests
{
	[Fact]
	public void Application_Request_ShouldNotExposeSetter()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();

		using (new AssertionScope(new AssertionStrategy()))
		{
			foreach (var assembly in assemblies)
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
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var applicationNamespace = typeof(IApplicationContractsAssemblyMarker).Namespace;

		using (new AssertionScope(new AssertionStrategy()))
		{
			foreach (var assembly in assemblies)
			{
				var types = AllTypes.From(assembly);

				types.ThatImplement<IRequest>()
					.Should()
					.BeUnderNamespace(applicationNamespace);
			}
		}
	}

	[Fact]
	public void Application_AllClassesEndsWithRequest_ShouldImplementIRequest()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes();

		using (new AssertionScope())
		{
			foreach (var type in types.Where(a => a.Name.EndsWith("Request") && a.Name != nameof(IRequest)))
			{
				type.Should()
					.Implement<IRequest>();
			}
		}
	}

	[Fact]
	public void Application_AllClassesImplementsIRequest_ShouldHaveRequestSuffix()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes();

		using (new AssertionScope())
		{
			foreach (var type in types.ThatImplement<IRequest>())
			{
				type.Name.Should()
					.EndWith("Request");
			}
		}
	}

	[Fact]
	public void Application_AllClassesImplementsIRequest_ShouldHaveCorrespondingValidator()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies
			.GetAllTypes()
			.ThatImplement<IRequest>();

		using (new AssertionScope(new AssertionStrategy()))
		{
			foreach (var type in types)
			{
				assemblies.SelectMany(a => a.GetTypes())
					.Where(a => a.Name == $"{type.Name}Validator")
					.Should()
					.NotBeNull("every Request '{0}' should have corresponding Validator", type);
			}
		}
	}
	
	[Fact]
	public void Application_AllClassesImplementsIRequest_ShouldHaveSingleCorrespondingValidator()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies
			.GetAllTypes()
			.ThatImplement<IRequest>();

		using (new AssertionScope(new AssertionStrategy()))
		{
			foreach (var type in types)
			{
				assemblies.SelectMany(a => a.GetTypes())
					.Count(a => a.Name == $"{type.Name}Validator")
					.Should()
					.Be(1, "every Request '{0}' should have single corresponding Validator", type);
			}
		}
	}
}
