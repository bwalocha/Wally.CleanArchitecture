using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using FluentAssertions;
using FluentAssertions.Types;
using Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft;
using Wally.CleanArchitecture.MicroService.WebApi;

namespace Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Helpers;

public static class TypeHelpers
{
	private static readonly List<string> _prefixes = new()
	{
		"Wally.CleanArchitecture.MicroService",
	};

	public static IEnumerable<Assembly> GetAllInternalAssemblies()
	{
		var assemblies = typeof(IInfrastructureDIMicrosoftAssemblyMarker).Assembly.GetReferencedAssemblies().Concat(
				typeof(Startup).Assembly.GetReferencedAssemblies())
			.Where(a => _prefixes.Exists(b => a.FullName.StartsWith(b)));

		foreach (var assembly in assemblies)
		{
			yield return Assembly.Load(assembly);
		}

		yield return typeof(Startup).Assembly;
	}

	public static Type GetTypeDefinitionIfGeneric(this Type type)
	{
		return type.IsGenericType ? type.GetGenericTypeDefinition() : type;
	}

	public static bool IsGenericType(this Type type, Type genericType)
	{
		return type.IsGenericType && type.GetGenericTypeDefinition() == genericType;
	}

	public static Type? GetGenericInterface(this Type type, Type genericInterface)
	{
		return type.IsGenericType(genericInterface)
			? type
			: type.GetInterfaces()
				.FirstOrDefault(t => t.IsGenericType(genericInterface));
	}

	public static bool ImplementsInterface(this Type type, Type interfaceType)
	{
		if (!interfaceType.IsInterface)
		{
			throw new ArgumentException($"Parameter '{nameof(interfaceType)}' is not an Interface");
		}

		foreach (var @interface in type.GetTypeInfo()
					.ImplementedInterfaces)
		{
			if (@interface == interfaceType)
			{
				return true;
			}
		}

		return false;
	}

	public static bool ImplementsGenericInterface(this Type type, Type interfaceType)
	{
		if (!interfaceType.IsInterface)
		{
			throw new ArgumentException($"Parameter '{nameof(interfaceType)}' is not an Interface");
		}

		if (type.IsGenericType(interfaceType))
		{
			return true;
		}

		foreach (var @interface in type.GetTypeInfo()
					.ImplementedInterfaces)
		{
			if (@interface.IsGenericType(interfaceType))
			{
				return true;
			}
		}

		return false;
	}

	public static TypeSelector GetAllExportedTypes(this IEnumerable<Assembly> assemblies)
	{
		return assemblies.SelectMany(a => a.GetExportedTypes())
			.Types();
	}
}
