using System.Linq;
using Wally.CleanArchitecture.MicroService.Application.Contracts;
using Wally.CleanArchitecture.MicroService.Application.Contracts.Abstractions;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Extensions;

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
			.Where(a => a.ImplementsInterface(typeof(IRequest)));
		
		types.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in types)
			{
				type.Namespace.ShouldStartWith(typeof(IApplicationContractsAssemblyMarker).Namespace!);
			}
		});
	}

	[Fact]
	public void Application_AllClassesEndsWithRequest_ShouldImplementIRequest()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.Name.EndsWith("Request") && a.Name != nameof(IRequest));

		types.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in types)
			{
				type.ImplementsInterface(typeof(IRequest))
					.ShouldBeTrue();
			}
		});
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
					.Count(a => a.Name == $"{type.Name}Validator")
					.ShouldBe(1, $"Request '{type}' should have single corresponding RequestValidator");
			}
		});
	}
}
