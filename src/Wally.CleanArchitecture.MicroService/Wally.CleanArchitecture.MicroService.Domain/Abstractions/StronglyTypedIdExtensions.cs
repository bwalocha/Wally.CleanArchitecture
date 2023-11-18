using System;

namespace Wally.CleanArchitecture.MicroService.Domain.Abstractions;

public static class StronglyTypedIdExtensions
{
	/// <summary>
	/// Checks the given type if it is an <see cref="StronglyTypedId{TStronglyTypedId,TValue}" />.
	/// </summary>
	/// <param name="type">The StronglyTypedId type.</param>
	/// <returns>True, if the type is an enumeration, false otherwise.</returns>
	public static bool IsStronglyTypedId(this Type? type)
	{
		if (type is null || type.IsAbstract || type.IsGenericTypeDefinition)
		{
			return false;
		}

		do
		{
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(StronglyTypedId<,>))
			{
				return true;
			}

			type = type.BaseType;
		}
		while (type is not null);

		return false;
	}

	/// <summary>
	/// Gets the type of the generic value parameter from the base type.
	/// </summary>
	/// <param name="type">The StronglyTypedId type.</param>
	/// <returns>The type of the value.</returns>
	public static Type? GetStronglyTypedIdValueType(this Type? type)
	{
		do
		{
			if (type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(StronglyTypedId<,>))
			{
				var valueType = type.GetGenericArguments()[1];
				return valueType;
			}

			type = type?.BaseType;
		}
		while (type is not null);

		return null;
	}
}
