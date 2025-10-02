using System;
using System.Linq;
using FluentValidation;
using Wally.CleanArchitecture.MicroService.Application;
using Wally.CleanArchitecture.MicroService.Application.Abstractions;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Extensions;
using Wally.CleanArchitecture.MicroService.WebApi;

namespace Wally.CleanArchitecture.MicroService.Tests.ConventionTests;

public class RequestTests
{
	[Fact]
	public void Application_Request_ShouldNotExposeSetter()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.ImplementsInterface(typeof(IRequest)));

		types.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in types)
			{
				foreach (var property in type.GetProperties())
				{
					if (property.SetMethod?.IsPublic != true)
					{
						continue;
					}

					property.IsPrivateWritable()
						.ShouldBeTrue($"Response class '{type}' should not expose setter for '{property.Name}'");
				}
			}
		});
	}

	[Fact]
	public void Application_ClassesWhichImplementsIRequest_ShouldBeInTheSameProject()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.ImplementsInterface(typeof(IRequest)))
			.Where(a => a.Assembly != typeof(IPresentationAssemblyMarker).Assembly);
		
		types.ShouldSatisfyAllConditions(types.Select(a => (Action)(() => a.Namespace.ShouldStartWith(typeof(IApplicationAssemblyMarker).Namespace!))).ToArray());
	}

	[Fact]
	public void Application_AllClassesEndsWithRequest_ShouldImplementIRequest()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.Name.EndsWith("Request"))
			.Where(a => a.Name != nameof(IRequest))
			.Where(a => a.Assembly != typeof(IPresentationAssemblyMarker).Assembly);

		types.ShouldSatisfyAllConditions(types.Select(a => (Action)(() => a.ImplementsInterface(typeof(IRequest)).ShouldBeTrue($"{a.Name} should implement {nameof(IRequest)}"))).ToArray());
	}

	[Fact]
	public void Application_AllClassesImplementsIRequest_ShouldHaveRequestSuffix()
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
	public void Application_AllClassesImplementsIRequest_ShouldHaveCorrespondingValidator()
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

	[Fact]
	public void Application_AllClassesImplementsIRequest_ShouldHaveSingleCorrespondingValidator()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies()
			.ToArray();
		var types = assemblies
			.GetAllTypes()
			.Where(a => a.ImplementsInterface(typeof(IRequest)));

		types.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in types)
			{
				assemblies.SelectMany(a => a.GetTypes())
					.Where(a => a.InheritsGenericClass(typeof(AbstractValidator<>)))
					.Where(a => a.BaseType?.GetGenericArguments().Single() == type)
					.Count(a => a.Name == $"{type.Name}Validator")
					.ShouldBe(1, $"Request '{type}' should have single corresponding RequestValidator");
			}
		});
	}
}
