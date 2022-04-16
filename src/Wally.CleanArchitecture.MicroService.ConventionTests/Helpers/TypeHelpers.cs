using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using FluentAssertions;
using FluentAssertions.Common;
using FluentAssertions.Types;

using Wally.CleanArchitecture.MicroService.WebApi;

namespace Wally.CleanArchitecture.MicroService.ConventionTests.Helpers;

public static class TypeHelpers
{
	private static readonly List<string> _prefixes = new() { "Wally.CleanArchitecture.MicroService", };

	public static IEnumerable<Assembly> GetAllInternalAssemblies()
	{
		var assemblies = typeof(Startup).Assembly.GetReferencedAssemblies()
			.Where(a => _prefixes.Any(b => a.FullName.StartsWith(b)));

		foreach (var assembly in assemblies)
		{
			yield return Assembly.Load(assembly);
		}

		yield return typeof(Startup).Assembly;
	}

	public static void HaveOnlyPrivateParameterlessConstructor(this TypeSelectorAssertions typeSelectorAssertions)
	{
		const string because = "entity '{0}' should have private parameterless constructor only.";

		foreach (var type in typeSelectorAssertions.Subject)
		{
			var constructors = type.GetConstructors();
			foreach (var constructor in constructors)
			{
				constructor.Should()
					.HaveAccessModifier(CSharpAccessModifier.Private, because, type);
				constructor.GetParameters()
					.Should()
					.BeEmpty(because, type);
			}

			type.Should()
				.HaveDefaultConstructor(because, type);
		}
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

	public static bool ImplementsGenericInterface(this Type type, Type interfaceType)
	{
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
}
