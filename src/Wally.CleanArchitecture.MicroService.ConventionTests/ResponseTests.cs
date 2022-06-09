using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;

using FluentAssertions;
using FluentAssertions.Common;
using FluentAssertions.Execution;
using FluentAssertions.Types;

using Wally.CleanArchitecture.MicroService.Contracts.Responses.Users;
using Wally.CleanArchitecture.MicroService.ConventionTests.Helpers;
using Wally.Lib.DDD.Abstractions.Responses;

using Xunit;

namespace Wally.CleanArchitecture.MicroService.ConventionTests;

public class ResponseTests
{
	[Fact]
	public void Application_ResponseWithParameterlessConstructor_ShouldNotExposeSetter()
	{
		using (new AssertionScope(new AssertionStrategy()))
		{
			foreach (var assembly in TypeHelpers.GetAllInternalAssemblies())
			{
				var types = AllTypes.From(assembly)
					.ThatImplement<IResponse>()
					.Where(a => a != typeof(PageInfoResponse))
					.Where(a => a.GetTypeDefinitionIfGeneric() != typeof(PagedResponse<>));

				foreach (var type in types)
				{
					if (type.GetConstructor(Type.EmptyTypes) == null)
					{
						continue;
					}

					foreach (var property in type.Properties())
					{
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
	public void Application_ResponseWithParametrizedConstructor_ShouldNotHaveSetter()
	{
		using (new AssertionScope(new AssertionStrategy()))
		{
			foreach (var assembly in TypeHelpers.GetAllInternalAssemblies())
			{
				var types = AllTypes.From(assembly)
					.ThatImplement<IResponse>()
					.Where(a => a != typeof(PageInfoResponse))
					.Where(a => a.GetTypeDefinitionIfGeneric() != typeof(PagedResponse<>));

				foreach (var type in types)
				{
					if (type.GetConstructor(Type.EmptyTypes) != null)
					{
						continue;
					}

					foreach (var property in type.Properties())
					{
						property.Should()
							.NotBeWritable(
								"Response class '{0}' should not have setter '{1}'",
								new object[] { type, property, });
					}
				}
			}
		}
	}

	[Fact]
	public void Application_ClassesWhichImplementsIResponse_ShouldBeInApplicationProject()
	{
		var applicationNamespace = Regex.Match(
				typeof(GetUserResponse).Namespace!,
				@"Wally.CleanArchitecture.MicroService\.Contracts\.Responses(?=[\.$])")
			.Value;

		using (new AssertionScope(new AssertionStrategy()))
		{
			foreach (var assembly in TypeHelpers.GetAllInternalAssemblies())
			{
				var types = AllTypes.From(assembly);

				types.ThatImplement<IResponse>()
					.ThatDoNotImplement<PageInfoResponse>()
					.Should()
					.BeUnderNamespace(applicationNamespace);
			}
		}
	}

	[Fact]
	public void Application_AllClassesEndsWithResponse_ShouldImplementIResponse()
	{
		using (new AssertionScope(new AssertionStrategy()))
		{
			foreach (var assembly in TypeHelpers.GetAllInternalAssemblies())
			{
				var types = AllTypes.From(assembly)
					.Where(a => a.Name.EndsWith("Response"));
				foreach (var type in types)
				{
					type.Should()
						.Implement<IResponse>();
				}
			}
		}
	}

	[Fact]
	public void Application_AllClassesImplementsIResponse_ShouldHasResponseSuffix()
	{
		using (new AssertionScope(new AssertionStrategy()))
		{
			foreach (var assembly in TypeHelpers.GetAllInternalAssemblies())
			{
				var types = AllTypes.From(assembly)
					.ThatImplement<IResponse>();
				foreach (var type in types)
				{
					type.Name.Should()
						.EndWith("Response", "Type '{0}' should ends with 'Response'", type);
				}
			}
		}
	}

	[Fact]
	public void Application_AllResponseObjects_ShouldBeExcludedFromCodeCoverage()
	{
		using (new AssertionScope(new AssertionStrategy()))
		{
			foreach (var assembly in TypeHelpers.GetAllInternalAssemblies())
			{
				var types = AllTypes.From(assembly)
					.ThatImplement<IResponse>();

				foreach (var type in types)
				{
					type.Should()
						.BeDecoratedWith<ExcludeFromCodeCoverageAttribute>();
				}
			}
		}
	}
}
