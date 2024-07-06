using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Types;
using Microsoft.AspNetCore.Mvc;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Extensions;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Helpers;
using Wally.Lib.DDD.Abstractions.Commands;
using Wally.Lib.DDD.Abstractions.Queries;
using Xunit;

namespace Wally.CleanArchitecture.MicroService.Tests.ConventionTests;

public class ControllerTests
{
	[Fact]
	public void Controller_Constructor_ShouldNotHaveICommandHandler()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();

		using (new AssertionScope())
		{
			foreach (var assembly in assemblies)
			{
				var types = AllTypes.From(assembly)
					.Where(a => a.BaseType == typeof(ControllerBase))
					.ToList();

				foreach (var type in types)
				{
					foreach (var constructor in type.GetConstructors())
					{
						foreach (var parameterInfo in constructor.GetParameters()
									.Select(parameterInfo => parameterInfo.ParameterType)
									.Where(parameterType => parameterType.IsGenericType))
						{
							parameterInfo.Should()
								.Match(
									a => a.GetGenericTypeDefinition() != typeof(ICommandHandler<>) &&
										a.GetGenericTypeDefinition() != typeof(ICommandHandler<,>),
									"Constructor of '{0}' should not take ICommandHandler as a parameter",
									type);
						}
					}
				}
			}
		}
	}

	[Fact]
	public void Controller_Constructor_ShouldntHaveIQueryHandler()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();

		using (new AssertionScope())
		{
			foreach (var assembly in assemblies)
			{
				var types = AllTypes.From(assembly)
					.Where(a => a.BaseType == typeof(ControllerBase))
					.ToList();

				foreach (var type in types)
				{
					foreach (var constructor in type.GetConstructors())
					{
						foreach (var parameterInfo in constructor.GetParameters()
									.Select(parameterInfo => parameterInfo.ParameterType)
									.Where(parameterType => parameterType.IsGenericType))
						{
							parameterInfo.Should()
								.NotBeAssignableTo(
									typeof(IQueryHandler<,>),
									"Constructor of '{0}' should not take IQueryHandler as a parameter",
									type);
						}
					}
				}
			}
		}
	}

	[Fact]
	public void Controller_ReturnType_ShouldBeOfTypeActionResult()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.ThatDeriveFrom<ControllerBase>()
			.ToList();

		using (new AssertionScope(new AssertionStrategy()))
		{
			foreach (var type in types)
			{
				foreach (var method in type.GetMethods(
							BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
				{
					method.ReturnType.Should()
						.BeDerivedFrom(typeof(Task<>), "controller '{0}', '{1}' should return Task", type, method);

					if (method.ReturnType.GenericTypeArguments.SingleOrDefault() == typeof(CreatedAtActionResult))
					{
						continue;
					}

					if (method.ReturnType.GenericTypeArguments.SingleOrDefault() == typeof(FileStreamResult))
					{
						continue;
					}

					method.ReturnType.GenericTypeArguments.SingleOrDefault()
						.Should()
						.BeDerivedFrom(
							typeof(ActionResult<>),
							"controller '{0}', '{1}' should return async ActionResult<>",
							type,
							method);
				}
			}
		}
	}
}
