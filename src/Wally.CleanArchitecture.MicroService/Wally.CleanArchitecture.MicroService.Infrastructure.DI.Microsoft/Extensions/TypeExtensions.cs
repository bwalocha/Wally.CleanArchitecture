using System;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Extensions;

public static class TypeExtensions
{
	public static bool IsGenericType(this Type type, Type genericType)
		=> type.IsGenericType && type.GetGenericTypeDefinition() == genericType;
}
