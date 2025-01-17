using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using FluentAssertions;
using FluentAssertions.Common;
using FluentAssertions.Execution;
using Wally.CleanArchitecture.MicroService.Application.Contracts;
using Wally.CleanArchitecture.MicroService.Application.Contracts.Abstractions;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Extensions;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Helpers;
using Xunit;

namespace Wally.CleanArchitecture.MicroService.Tests.ConventionTests;

public class ResponseTests
{
	[Fact]
	public void Application_ResponseWithParameterlessConstructor_ShouldNotExposeSetter()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.ThatImplement<IResponse>();

		using var scope = new AssertionScope(new AssertionStrategy());
		foreach (var type in types)
		{
			if (type.GetConstructor(Type.EmptyTypes) == null)
			{
				// Check only classes with non parametrized ctors
				continue;
			}

			foreach (var property in type.Properties())
			{
				var isInitOnly = (property.GetSetMethod()
					?.ReturnParameter.GetRequiredCustomModifiers()
					.Length ?? 0) > 0;
				if (isInitOnly)
				{
					var isRequired = property.GetCustomAttribute(typeof(RequiredMemberAttribute)) != null;

					if (isRequired)
					{
						continue;
					}

					property.Should()
						.BeDecoratedWith<RequiredMemberAttribute>(
							"Response class '{0}' with Init setter should have Required modifier '{1}'", type,
							property);
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

	[Fact]
	public void Application_ResponseWithParametrizedConstructor_ShouldNotHaveSetter()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.ThatImplement<IResponse>();

		using var scope = new AssertionScope(new AssertionStrategy());
		foreach (var type in types)
		{
			if (type.GetConstructor(Type.EmptyTypes) != null && type.GetConstructors()
					.Length == 1)
			{
				// Check only classes with parametrized ctors 
				continue;
			}

			foreach (var property in type.Properties())
			{
				property.Should()
					.NotBeWritable(
						"Response class '{0}' should not have setter '{1}'", type, property);
			}
		}
	}

	[Fact]
	public void Application_ClassesWhichImplementsIResponse_ShouldBeInApplicationContractsProject()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var applicationNamespace = typeof(IApplicationContractsAssemblyMarker).Namespace;
		var types = assemblies.GetAllTypes();

		using var scope = new AssertionScope(new AssertionStrategy());
		types.ThatImplement<IResponse>()
			.Should()
			.BeUnderNamespace(applicationNamespace);
	}

	[Fact]
	public void Application_AllClassesEndsWithResponse_ShouldImplementIResponse()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.IsClass)
			.Where(a => a.Name.EndsWith("Response"));

		using var scope = new AssertionScope(new AssertionStrategy());
		foreach (var type in types)
		{
			type.Should()
				.Implement<IResponse>();
		}
	}

	[Fact]
	public void Application_AllClassesImplementsIResponse_ShouldHasResponseSuffix()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.ThatImplement<IResponse>();

		using var scope = new AssertionScope(new AssertionStrategy());
		foreach (var type in types)
		{
			type.Name.Split('`')[0]
				.Should()
				.EndWith("Response", "Type '{0}' should ends with 'Response'", type);
		}
	}

	[Fact]
	public void Application_AllResponseObjects_ShouldBeExcludedFromCodeCoverage()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.ThatImplement<IResponse>();

		using var scope = new AssertionScope(new AssertionStrategy());
		foreach (var type in types)
		{
			type.Should()
				.BeDecoratedWith<ExcludeFromCodeCoverageAttribute>();
		}
	}
}
