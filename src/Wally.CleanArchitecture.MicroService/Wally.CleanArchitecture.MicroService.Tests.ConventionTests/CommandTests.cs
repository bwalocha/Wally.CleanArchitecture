using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FluentValidation;
using Shouldly;
using Wally.CleanArchitecture.MicroService.Application.Abstractions;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Extensions;
using Xunit;

namespace Wally.CleanArchitecture.MicroService.Tests.ConventionTests;

public class CommandTests
{
	[Fact]
	public void Application_Command_ShouldHaveNameSuffix()
	{
		var applicationTypes = Configuration.Assemblies.Application.GetAllTypes();
		var types = applicationTypes
			.Where(a => a.IsClass)
			.Where(a => !a.IsAbstract)
			.Where(a => a.ImplementsGenericInterface(typeof(ICommand<>)));

		types.ShouldSatisfyAllConditions(
			() =>
			{
				foreach (var type in types)
				{
					type.Name
						.ShouldEndWith("Command", Case.Sensitive, $"Command '{type}' name should end with 'Command'");
				}
			});
	}

	[Fact]
	public void Application_Command_ShouldNotExposeSetter()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.ImplementsInterface(typeof(ICommand)) || a.ImplementsGenericInterface(typeof(ICommand<>)));

		types.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in types)
			{
				foreach (var property in type.GetProperties())
				{
					property.GetSetMethod()
						.ShouldBeNull($"Command '{type}' and property '{property}' should be immutable");
				}
			}
		});
	}

	[Fact]
	public void Application_Command_ShouldBeExcludedFromCodeCoverage()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.IsClass)
			.Where(a => a.ImplementsInterface(typeof(ICommand)) ||
				a.ImplementsGenericInterface(typeof(ICommand<>)));

		types.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in types)
			{
				type.ShouldBeDecoratedWith<ExcludeFromCodeCoverageAttribute>();
			}
		});
	}
	
	[Fact]
	public void Application_Command_ShouldBeSealed()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.IsClass)
			.Where(a => a.ImplementsInterface(typeof(ICommand)) ||
				a.ImplementsGenericInterface(typeof(ICommand<>)));

		types.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in types)
			{
				type.IsSealed
					.ShouldBeTrue($"command '{type}' should be sealed");
			}
		});
	}

	[Fact]
	public void Application_Command_ShouldHaveCorrespondingHandler()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies()
			.ToList();
		var types = assemblies.GetAllTypes()
			.Where(a => a.IsClass)
			.Where(a => a.ImplementsInterface(typeof(ICommand)) ||
				a.ImplementsGenericInterface(typeof(ICommand<>)));

		assemblies.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in types)
			{
				assemblies.GetAllTypes()
					.SingleOrDefault(a => a.Name == $"{type.Name}Handler")
					.ShouldNotBeNull($"Command '{type}' should have corresponding Handler");
			}
		});
	}

	[Fact]
	public void Application_Command_ShouldHaveCorrespondingValidator()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies()
			.ToArray();
		var types = assemblies.GetAllTypes()
			.Where(a => a.IsClass)
			.Where(a => a.ImplementsInterface(typeof(ICommand)) ||
				a.ImplementsGenericInterface(typeof(ICommand<>)));

		types.ShouldSatisfyAllConditions(
			() =>
			{
				foreach (var type in types)
				{
					var expectedBaseType = typeof(AbstractValidator<>).MakeGenericType(type);

					var subject = assemblies.SelectMany(a => a.GetTypes())
						.SingleOrDefault(a => a.Name == $"{type.Name}Validator");

					subject.ShouldNotBeNull($"Command '{type}' should have corresponding Validator");

					subject.InheritsClass(expectedBaseType)
						.ShouldBeTrue($"Command '{type}' should inherits {expectedBaseType} base class");
				}
			});
	}

	[Fact]
	public void Application_Command_ShouldHaveCorrespondingAuthorizationHandler()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies()
			.ToArray();
		var types = assemblies.GetAllTypes()
			.Where(a => a.IsClass)
			.Where(a => a.ImplementsInterface(typeof(ICommand)) ||
				a.ImplementsGenericInterface(typeof(ICommand<>)));
		
		types.ShouldSatisfyAllConditions(() =>
		{
				foreach (var type in types)
				{
					assemblies.GetAllTypes()
						.SingleOrDefault(a => a.Name == $"{type.Name}AuthorizationHandler")
						.ShouldNotBeNull($"Command '{type}' should have corresponding AuthorizationHandler");
				}
		});
	}
}
