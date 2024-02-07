using System;

namespace Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Extensions;

public static class TypeExtensions
{
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
}
