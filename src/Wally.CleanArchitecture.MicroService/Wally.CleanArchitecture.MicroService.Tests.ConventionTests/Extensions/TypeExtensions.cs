using System;
using System.Reflection;
using System.Linq;

namespace Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Extensions;

public static class TypeExtensions
{
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
	
	public static bool InheritsGenericClass(this Type type, Type classType)
	{
		if (!classType.IsClass)
		{
			throw new ArgumentException($"Parameter '{nameof(classType)}' is not a Class");
		}

		while (type != null && type != typeof(object))
		{
			var current = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
			if (classType == current)
			{
				return true;
			}

			if (type.BaseType == null)
			{
				break;
			}

			type = type.BaseType;
		}

		return false;
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
}
