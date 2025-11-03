using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Extensions;
using Wally.CleanArchitecture.MicroService.WebApi;
using Wally.CleanArchitecture.MicroService.WebApi.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Presentation;

public class ResponseTests
{
	[Fact]
	public void Presentation_Response_ShouldBeSealed()
	{
		// Arrange
		IArchRule rule =
			Classes()
				.That()
				.Are(Configuration.PresentationProvider)
				.And()
				.ImplementInterface(typeof(IResponse))
				.And()
				.AreNot(typeof(PagedResponse<>))
				.Should()
				.BeSealed()
				.Because("Presentation Responses should be Sealed.");

		// Act

		// Assert
		rule.Check(Configuration.Architecture);
	}
	
	[Fact]
	public void Presentation_Response_ShouldNotExposeSetter()
	{
		// Arrange
		var responses = Classes()
			.That()
			.Are(Configuration.PresentationProvider)
			.And()
			.ImplementInterface(typeof(IResponse));
		IArchRule rule = Members()
			.That()
			.AreDeclaredIn(responses)
			.Should()
			.HaveOnlyInitSetters()
			.Because("Presentation Responses should be immutable.");

		// Act

		// Assert
		rule.Check(Configuration.Architecture);
	}
	
	[Fact]
	public void Presentation_Response_ShouldHaveNameConvention()
	{
		// Arrange
		IArchRule rule =
				Classes()
					.That()
					.Are(Configuration.PresentationProvider)
					.And()
					.ImplementInterface(typeof(IResponse))
					.And()
					.AreNot(typeof(PagedResponse<>))
				.Should()
				.HaveNameEndingWith("Response")
				.Because("Presentation Responses should have name convention.");

		// Act

		// Assert
		rule.Check(Configuration.Architecture);
	}

	[Fact]
	public void Presentation_ResponseWithParameterlessConstructor_ShouldNotExposeSetter()
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
							$"Response class '{type}' with Init setter should have Required modifier for '{property}'");
					}

					if (!property.CanWrite)
					{
						continue;
					}
					
					property.IsPrivateWritable()
						.ShouldBeTrue($"Response class '{type}' should not expose setter for '{property}'");
				}
			}
		});
	}

	[Fact]
	public void Presentation_ResponseWithParametrizedConstructor_ShouldNotHaveSetter()
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
	public void Presentation_ClassesWhichImplementsIResponse_ShouldBeInApplicationContractsProject()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.ImplementsInterface(typeof(IResponse)));

		types.ShouldSatisfyAllConditions(types.Select(a => (Action)(() => a.Namespace.ShouldStartWith(typeof(IPresentationAssemblyMarker).Namespace!))).ToArray());
	}

	[Fact]
	public void Presentation_AllClassesEndsWithResponse_ShouldImplementIResponse()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.IsClass)
			.Where(a => a.Name.EndsWith("Response"))
			.Where(a => a.Assembly != typeof(IPresentationAssemblyMarker).Assembly);

		types.ShouldSatisfyAllConditions(types.Select(a => (Action)(() => a.ImplementsInterface(typeof(IResponse)).ShouldBeTrue($"{a.Name} should implement {nameof(IResponse)}"))).ToArray());
	}

	[Fact]
	public void Presentation_AllClassesImplementsIResponse_ShouldHasResponseSuffix()
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
