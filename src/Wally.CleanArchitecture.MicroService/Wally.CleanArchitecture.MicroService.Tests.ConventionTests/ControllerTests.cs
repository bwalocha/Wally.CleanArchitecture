using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Wally.CleanArchitecture.MicroService.Application.Abstractions;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Extensions;

namespace Wally.CleanArchitecture.MicroService.Tests.ConventionTests;

public class ControllerTests
{
	[Fact]
	public void Controller_Constructor_ShouldNotHaveICommandHandler()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var controllerTypes = assemblies.SelectMany(a => a.GetTypes())
			.Where(a => a.InheritsClass(typeof(ControllerBase)));

		assemblies.ShouldSatisfyAllConditions(() => 
		{
			foreach (var type in controllerTypes)
			{
				foreach (var constructor in type.GetConstructors())
				{
					foreach (var parameterInfo in constructor.GetParameters()
								.Select(parameterInfo => parameterInfo.ParameterType)
								.Where(parameterType => parameterType.IsGenericType))
					{
						parameterInfo.GetGenericTypeDefinition()
							.ShouldNotBe(typeof(ICommandHandler<>),
								$"Constructor of '{type}' should not take 'ICommandHandler' as a parameter");
						parameterInfo.GetGenericTypeDefinition()
							.ShouldNotBe(typeof(ICommandHandler<,>),
								$"Constructor of '{type}' should not take 'ICommandHandler' as a parameter");
					}
				}
			}
		});
	}

	[Fact]
	public void Controller_Constructor_ShouldNotHaveIQueryHandler()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var controllerTypes = assemblies.SelectMany(a => a.GetTypes())
			.Where(a => a.InheritsClass(typeof(ControllerBase)));

		assemblies.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in controllerTypes)
			{
				foreach (var constructor in type.GetConstructors())
				{
					foreach (var parameterInfo in constructor.GetParameters()
								.Select(parameterInfo => parameterInfo.ParameterType)
								.Where(parameterType => parameterType.IsGenericType))
					{
						parameterInfo.ShouldNotBeAssignableTo(
								typeof(IQueryHandler<,>),
								$"Constructor of '{type}' should not take 'IQueryHandler' as a parameter");
					}
				}
			}
		});
	}

	[Fact]
	public void Controller_ReturnType_ShouldBeOfTypeActionResult()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var controllerTypes = assemblies.SelectMany(a => a.GetTypes())
			.Where(a => a.InheritsClass(typeof(ControllerBase)));

		assemblies.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in controllerTypes)
			{
				foreach (var method in type.GetMethods(
							BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
				{
					method.ReturnType.InheritsGenericClass(typeof(Task<>))
						.ShouldBeTrue($"Controller '{type}' and '{method}' method return type should be 'Task'");

					if (method.ReturnType.GenericTypeArguments.SingleOrDefault() == typeof(CreatedAtActionResult))
					{
						continue;
					}

					if (method.ReturnType.GenericTypeArguments.SingleOrDefault() == typeof(FileStreamResult))
					{
						continue;
					}

					method.ReturnType.GenericTypeArguments
						.SingleOrDefault(a => a.InheritsGenericClass(typeof(ActionResult)) || a.InheritsGenericClass(typeof(ActionResult<>)))
						.ShouldNotBeNull(
							$"Controller '{type}' and '{method}' method return type should be 'async ActionResult<>'");
				}
			}
		});
	}
}
