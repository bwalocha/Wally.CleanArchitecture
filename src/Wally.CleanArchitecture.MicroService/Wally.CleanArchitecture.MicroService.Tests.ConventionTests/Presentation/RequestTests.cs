using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Wally.CleanArchitecture.MicroService.Application;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Extensions;
using Wally.CleanArchitecture.MicroService.WebApi;
using Wally.CleanArchitecture.MicroService.WebApi.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Presentation;

public class RequestTests
{
	[Fact]
	public void Presentation_Request_ShouldBePublic()
	{
		// Arrange
		IArchRule rule = Classes()
			.That()
			.Are(Configuration.PresentationProvider)
			.And()
			.ImplementInterface(typeof(IRequest))
			.Should()
			.BePublic()
			.Because("Presentation Requests should be public.");

		// Act

		// Assert
		rule.Check(Configuration.Architecture);
	}
	
	[Fact]
	public void Presentation_Request_ShouldBeSealed()
	{
		// Arrange
		IArchRule rule =
			Classes()
				.That()
				.Are(Configuration.PresentationProvider)
				.And()
				.ImplementInterface(typeof(IRequest))
				.Should()
				.BeSealed()
				.Because("Presentation Requests should be Sealed.");

		// Act

		// Assert
		rule.Check(Configuration.Architecture);
	}
	
	[Fact]
	public void Presentation_Request_ShouldNotExposeSetter()
	{
		// Arrange
		var requests = Classes()
			.That()
			.Are(Configuration.PresentationProvider)
			.And()
			.ImplementInterface(typeof(IRequest));
		IArchRule rule = Members()
			.That()
			.AreDeclaredIn(requests)
			.Should()
			.HaveOnlyInitSetters()
			.Because("Presentation Requests should be immutable.");
		
		// Act
		
		// Assert
		rule.Check(Configuration.Architecture);
		
		// Arrange
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.ImplementsInterface(typeof(IRequest)));

		// Act
		void Act(Type type, PropertyInfo property)
		{
			if (property.SetMethod?.IsPublic != true)
			{
				return;
			}

			var isInitOnly = (property.GetSetMethod()
				?.ReturnParameter.GetRequiredCustomModifiers()
				.Length ?? 0) > 0;
			if (isInitOnly)
			{
				var isRequired = property.GetCustomAttribute(typeof(RequiredMemberAttribute)) != null;

				if (isRequired)
				{
					return;
				}

				property.PropertyType.ShouldBeDecoratedWith<RequiredMemberAttribute>(
					$"Request class '{type}' with Init setter should have Required modifier for '{property}'");
			}

			if (!property.CanWrite)
			{
				return;
			}
					
			property.IsPrivateWritable()
				.ShouldBeTrue($"Request class '{type}' should not expose setter for '{property}'");
		}

		// Assert
		types.ShouldSatisfyAllConditions(types.SelectMany(a => a.GetProperties().Select(b => new
			{
				Type = a,
				Property = b,
			}))
			.Select(a => (Action)(() => Act(a.Type, a.Property)))
			.ToArray());
	}

	[Fact]
	public void Presentation_Request_ShouldHaveNameConvention()
	{
		// Arrange
		IArchRule rule =
			Classes()
				.That()
				.Are(Configuration.PresentationProvider)
				.And()
				.ImplementInterface(typeof(IRequest))
				.Should()
				.HaveNameEndingWith("Request")
				.Because("Presentation Requests should have name convention.");

		// Act

		// Assert
		rule.Check(Configuration.Architecture);
	}
	
	[Fact]
	public void Presentation_ClassesWhichImplementsIRequest_ShouldBeInTheSameProject()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.ImplementsInterface(typeof(IRequest)))
			.Where(a => a.Assembly != typeof(IPresentationAssemblyMarker).Assembly);
		
		types.ShouldSatisfyAllConditions(types.Select(a => (Action)(() => a.Namespace.ShouldStartWith(typeof(IPresentationAssemblyMarker).Namespace!))).ToArray());
	}

	[Fact]
	public void Presentation_AllClassesEndsWithRequest_ShouldImplementIRequest()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.Name.EndsWith("Request"))
			.Where(a => a.Name != nameof(IRequest))
			.Where(a => a.Assembly != typeof(IApplicationAssemblyMarker).Assembly);

		types.ShouldSatisfyAllConditions(types.Select(a => (Action)(() => a.ImplementsInterface(typeof(IRequest)).ShouldBeTrue($"{a.Name} should implement {nameof(IRequest)}"))).ToArray());
	}

	[Fact]
	public void Presentation_AllClassesImplementsIRequest_ShouldHaveRequestSuffix()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.ImplementsInterface(typeof(IRequest)));

		types.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in types)
			{
				type.Name.ShouldEndWith("Request");
			}
		});
	}

	[Fact]
	public void Presentation_AllClassesImplementsIRequest_ShouldHaveCorrespondingValidator()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies().ToArray();
		var types = assemblies
			.GetAllTypes()
			.Where(a => a.ImplementsInterface(typeof(IRequest)));

		types.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in types)
			{
				assemblies.SelectMany(a => a.GetTypes())
					.Where(a => a.Name == $"{type.Name}Validator")
					.ShouldNotBeNull($"Request '{type}' should have corresponding RequestValidator");
			}
		});
	}
}
