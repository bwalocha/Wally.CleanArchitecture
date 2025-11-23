using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Extensions;

public static class PropertyInfoExtensions
{
	/// <summary>
	///     Determines if this property is marked as init-only.
	/// </summary>
	/// <param name="property">The property.</param>
	/// <returns>True if the property is init-only, false otherwise.</returns>
	public static bool IsInitOnly(this PropertyInfo property)
	{
		if (!property.CanWrite)
		{
			return false;
		}

		var setMethod = property.SetMethod;
		if (setMethod == null)
		{
			return true;
		}

		// Get the modifiers applied to the return parameter.
		var setMethodReturnParameterModifiers = setMethod.ReturnParameter.GetRequiredCustomModifiers();

		// Init-only properties are marked with the IsExternalInit type.
		return setMethodReturnParameterModifiers.Contains(typeof(IsExternalInit));
	}

	public static bool IsPrivateWritable(this PropertyInfo property)
	{
		var setMethod = property.GetSetMethod(true);
		return setMethod != null && setMethod.IsPrivate && !property.IsInitOnly();
	}
}
