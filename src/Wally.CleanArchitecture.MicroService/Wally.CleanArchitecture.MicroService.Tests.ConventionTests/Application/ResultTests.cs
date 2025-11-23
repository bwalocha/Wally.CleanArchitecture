using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Wally.CleanArchitecture.MicroService.Application;
using Wally.CleanArchitecture.MicroService.Application.Abstractions;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Extensions;
using Wally.CleanArchitecture.MicroService.WebApi;

namespace Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Application;

public class ResultTests
{
	[Fact]
	public void Application_Result_ShouldBePublic()
	{
		// Arrange
		IArchRule rule = Classes()
			.That()
			.Are(Configuration.ApplicationProvider)
			.And()
			.ImplementInterface(typeof(IResult))
			.Should()
			.BePublic()
			.Because("Application Results should be public.");

		// Act

		// Assert
		rule.Check(Configuration.Architecture);
	}

	[Fact]
	public void Application_Result_ShouldNotExposeSetter()
	{
		// Arrange
		var requests = Classes()
			.That()
			.Are(Configuration.ApplicationProvider)
			.And()
			.ImplementInterface(typeof(IResult));
		IArchRule rule =
			Members()
				.That()
				.AreDeclaredIn(requests)
				.Should()
				.HaveOnlyInitSetters()
				.Because("Application Results should be immutable.");

		// Act

		// Assert
		rule.Check(Configuration.Architecture);
	}

	[Fact]
	public void Application_ResultWithParameterlessConstructor_ShouldNotExposeSetter()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.ImplementsInterface(typeof(IResult)));

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
							$"Result class '{type}' with Init setter should have Required modifier for '{property}'");
					}

					if (!property.CanWrite)
					{
						continue;
					}

					property.IsPrivateWritable()
						.ShouldBeTrue($"Result class '{type}' should not expose setter for '{property}'");
				}
			}
		});
	}

	[Fact]
	public void Application_ResultWithParametrizedConstructor_ShouldNotHaveSetter()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.ImplementsInterface(typeof(IResult)));

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
						.ShouldBeNull($"Result class '{type}' should not have setter '{property}'");
				}
			}
		});
	}

	[Fact]
	public void Application_ClassesWhichImplementsIResult_ShouldBeInApplicationContractsProject()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.ImplementsInterface(typeof(IResult)));

		types.ShouldSatisfyAllConditions(types
			.Select(a => (Action)(() => a.Namespace.ShouldStartWith(typeof(IApplicationAssemblyMarker).Namespace!)))
			.ToArray());
	}

	[Fact]
	public void Application_AllClassesEndsWithResult_ShouldImplementIResult()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.IsClass)
			.Where(a => a.Name.EndsWith("Result"))
			.Where(a => a.Assembly != typeof(IPresentationAssemblyMarker).Assembly);

		types.ShouldSatisfyAllConditions(types.Select(a => (Action)(() => a.ImplementsInterface(typeof(IResult))
				.ShouldBeTrue($"{a.Name} should implement {nameof(IResult)}")))
			.ToArray());
	}

	[Fact]
	public void Application_AllClassesImplementsIResult_ShouldHasResultSuffix()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.ImplementsInterface(typeof(IResult)));

		types.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in types)
			{
				type.Name.Split('`')[0]
					.ShouldEndWith("Result", Case.Sensitive, $"Result '{type}' should end with 'Result'");
			}
		});
	}

	[Fact]
	public void Application_AllResultObjects_ShouldBeExcludedFromCodeCoverage()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.ImplementsInterface(typeof(IResult)));

		types.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in types)
			{
				type.ShouldBeDecoratedWith<ExcludeFromCodeCoverageAttribute>();
			}
		});
	}
}
