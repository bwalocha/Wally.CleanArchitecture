using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Extensions;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Helpers;
using Wally.Lib.DDD.Abstractions.Commands;
using Xunit;

namespace Wally.CleanArchitecture.MicroService.Tests.ConventionTests;

public class CommandTests
{
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
			.Where(a => a.ImplementsInterface(typeof(ICommand)) || a.ImplementsGenericInterface(typeof(ICommand<>)));

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
		var assemblies = Configuration.Assemblies.GetAllAssemblies();

		using (new AssertionScope(new AssertionStrategy()))
		{
			foreach (var assembly in assemblies)
			{
				var types = assembly.GetTypes()
					.Where(a => a.ImplementsInterface(typeof(ICommand)) || a.ImplementsGenericInterface(typeof(ICommand<>)))
					.Where(a => a.IsClass);
				
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
		var assemblies = Configuration.Assemblies.GetAllAssemblies();

		using (new AssertionScope(new AssertionStrategy()))
		{
			foreach (var assembly in assemblies)
			{
				var types = assembly.GetTypes()
					.Where(a => a.ImplementsInterface(typeof(ICommand)) || a.ImplementsGenericInterface(typeof(ICommand<>)))
					.Where(a => a.IsClass);
				
				foreach (var type in types)
				{
					assemblies.SelectMany(a => a.GetTypes())
						.SingleOrDefault(a => a.Name == $"{type.Name}Validator")
						.Should()
						.NotBeNull("Command '{0}' should have corresponding Validator", type);
				}
			}
		}
	}

	[Fact]
	public void Application_Command_ShouldBeSealed()
	{
		var applicationTypes = Configuration.Assemblies.Application.GetAllTypes();
		var types = applicationTypes
			.Where(a => a.ImplementsInterface(typeof(ICommand)) || a.ImplementsGenericInterface(typeof(ICommand<>)))
			.Where(a => a.IsClass);

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
