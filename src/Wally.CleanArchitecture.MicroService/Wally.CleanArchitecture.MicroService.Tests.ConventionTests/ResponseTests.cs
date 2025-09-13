using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Wally.CleanArchitecture.MicroService.Application.Contracts;
using Wally.CleanArchitecture.MicroService.Application.Contracts.Abstractions;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Extensions;

namespace Wally.CleanArchitecture.MicroService.Tests.ConventionTests;

public class ResponseTests
{
	[Fact]
	public void Application_ResponseWithParameterlessConstructor_ShouldNotExposeSetter()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.ImplementsInterface(typeof(IResponse)));

		types.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in types)
			{
				if (type.GetConstructor(Type.EmptyTypes) == null)
				{
					// Check only classes with non parametrized ctors
					continue;
				}

				foreach (var property in type.GetProperties())
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

						property.PropertyType.ShouldBeDecoratedWith<RequiredMemberAttribute>(
							$"Response class '{type}' with Init setter should have Required modifier '{property}'");
					}

					if (!property.CanWrite)
					{
						continue;
					}
					
					property.IsPrivateWritable()
						.ShouldBeTrue($"Response class '{type}' should not expose setter '{property}'");
				}
			}
		});
	}

	[Fact]
	public void Application_ResponseWithParametrizedConstructor_ShouldNotHaveSetter()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.ImplementsInterface(typeof(IResponse)));

		types.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in types)
			{
				if (type.GetConstructor(Type.EmptyTypes) != null && type.GetConstructors()
						.Length == 1)
				{
					// Check only classes with parametrized ctors 
					continue;
				}

				foreach (var property in type.GetProperties())
				{
					property.GetSetMethod()
						.ShouldBeNull($"Response class '{type}' should not have setter '{property}'");
				}
			}
		});
	}

	[Fact]
	public void Application_ClassesWhichImplementsIResponse_ShouldBeInApplicationContractsProject()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.ImplementsInterface(typeof(IResponse)));

		types.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in types)
			{
				type.Namespace.ShouldStartWith(typeof(IApplicationContractsAssemblyMarker).Namespace!);
			}
		});
	}

	[Fact]
	public void Application_AllClassesEndsWithResponse_ShouldImplementIResponse()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.IsClass)
			.Where(a => a.Name.EndsWith("Response"));

		types.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in types)
			{
				type.ImplementsInterface(typeof(IResponse)).ShouldBeTrue();
			}
		});
	}

	[Fact]
	public void Application_AllClassesImplementsIResponse_ShouldHasResponseSuffix()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.ImplementsInterface(typeof(IResponse)));

		types.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in types)
			{
				type.Name.Split('`')[0]
					.ShouldEndWith("Response", Case.Sensitive, $"Response '{type}' should end with 'Response'");
			}
		});
	}

	[Fact]
	public void Application_AllResponseObjects_ShouldBeExcludedFromCodeCoverage()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.ImplementsInterface(typeof(IResponse)));

		types.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in types)
			{
				type.ShouldBeDecoratedWith<ExcludeFromCodeCoverageAttribute>();
			}
		});
	}
}
