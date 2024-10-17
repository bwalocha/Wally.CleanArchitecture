using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentValidation;
using Wally.CleanArchitecture.MicroService.Application.Abstractions;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Extensions;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Helpers;
using Xunit;

namespace Wally.CleanArchitecture.MicroService.Tests.ConventionTests;

public class CommandTests
{
	[Fact]
	public void Application_Command_ShouldEndsWithCommand()
	{
		var applicationTypes = Configuration.Assemblies.Application.GetAllTypes();
		var types = applicationTypes
			.Where(a => a.IsClass)
			.Where(a => !a.IsAbstract)
			.Where(a => a.ImplementsGenericInterface(typeof(ICommand<>)));

		using (new AssertionScope(new AssertionStrategy()))
		{
			foreach (var type in types)
			{
				type.Name
					.Should()
					.EndWith("Command", "All Command names should end with 'Command'");
			}
		}
	}
	
	[Fact]
	public void Application_Command_ShouldNotExposeSetter()
	{
		var applicationTypes = Configuration.Assemblies.Application.GetAllTypes();
		var types = applicationTypes
			.Where(a => a.ImplementsInterface(typeof(ICommand)) || a.ImplementsGenericInterface(typeof(ICommand<>)));

		using (new AssertionScope(new AssertionStrategy()))
		{
			foreach (var type in types)
			{
				type
					.Properties()
					.Should()
					.NotBeWritable("commands should be immutable");
			}
		}
	}

	[Fact]
	public void Application_Command_ShouldBeExcludedFromCodeCoverage()
	{
		var applicationTypes = Configuration.Assemblies.Application.GetAllTypes();
		var types = applicationTypes
			.Where(a => a.IsClass)
			.Where(a => a.ImplementsInterface(typeof(ICommand)) ||
				a.ImplementsGenericInterface(typeof(ICommand<>)));

		using (new AssertionScope(new AssertionStrategy()))
		{
			foreach (var type in types)
			{
				type
					.Should()
					.BeDecoratedWith<ExcludeFromCodeCoverageAttribute>();
			}
		}
	}

	[Fact]
	public void Application_Command_ShouldHaveCorrespondingHandler()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies().ToList();

		using (new AssertionScope(new AssertionStrategy()))
		{
			foreach (var assembly in assemblies)
			{
				var types = assembly.GetTypes()
					.Where(a => a.IsClass)
					.Where(a => a.ImplementsInterface(typeof(ICommand)) ||
						a.ImplementsGenericInterface(typeof(ICommand<>)));

				foreach (var type in types)
				{
					assemblies.SelectMany(a => a.GetTypes())
						.SingleOrDefault(a => a.Name == $"{type.Name}Handler")
						.Should()
						.NotBeNull("Command '{0}' should have corresponding Handler", type);
				}
			}
		}
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

		using (new AssertionScope(new AssertionStrategy()))
		{
			foreach (var type in types)
			{
				var expectedBaseType = typeof(AbstractValidator<>).MakeGenericType(type);

				var subject = assemblies.SelectMany(a => a.GetTypes())
					.SingleOrDefault(a => a.Name == $"{type.Name}Validator");

				subject.Should()
					.NotBeNull("Command '{0}' should have corresponding Validator", type);

				subject!.InheritsClass(expectedBaseType)
					.Should()
					.BeTrue("Command '{0}' should inherits {1} base class", type, expectedBaseType);
			}
		}
	}

	[Fact]
	public void Application_Command_ShouldHaveCorrespondingAuthorizationHandler()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies().ToList();

		using (new AssertionScope(new AssertionStrategy()))
		{
			foreach (var assembly in assemblies)
			{
				var types = assembly.GetTypes()
					.Where(a => a.IsClass)
					.Where(a => a.ImplementsInterface(typeof(ICommand)) ||
						a.ImplementsGenericInterface(typeof(ICommand<>)));

				foreach (var type in types)
				{
					assembly.GetTypes()
						.SingleOrDefault(a => a.Name == $"{type.Name}AuthorizationHandler")
						.Should()
						.NotBeNull("Command '{0}' should have corresponding AuthorizationHandler", type);
				}
			}
		}
	}
	
	[Fact]
	public void Application_Command_ShouldBeSealed()
	{
		var applicationTypes = Configuration.Assemblies.Application.GetAllTypes();
		var types = applicationTypes
			.Where(a => a.IsClass)
			.Where(a => a.ImplementsInterface(typeof(ICommand)) ||
				a.ImplementsGenericInterface(typeof(ICommand<>)));

		using (new AssertionScope(new AssertionStrategy()))
		{
			foreach (var type in types)
			{
				type
					.Should()
					.BeSealed("commands should be sealed");
			}
		}
	}
}
