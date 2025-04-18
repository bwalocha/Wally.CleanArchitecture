using System;
using System.Collections;
using System.Reflection;
using Shouldly;

namespace Wally.CleanArchitecture.MicroService.Tests.UnitTests.Extensions;

public static class ShouldlyExtensions
{
	public static void ShouldBeLike(this object? actual, object? expected)
	{
		if (expected is null)
		{
			actual.ShouldBeNull();
			return;
		}
		
		if (actual is null)
		{
			throw new ShouldAssertException(new ExpectedEquivalenceShouldlyMessage(expected, actual, [], "One is null").ToString());
		}
		
		if (IsSimpleType(expected.GetType()))
		{
			actual.ShouldBe(expected);
			return;
		}

		if (expected is IEnumerable)
		{
			var actualEnumerator = ((IEnumerable)actual).GetEnumerator();
			foreach (var expectedValue in expected as IEnumerable)
			{
				var actualValue = actualEnumerator.MoveNext() ? actualEnumerator.Current : null;
				actualValue.ShouldBeLike(expectedValue);
			}

			return;
		}
		
		var expectedProperties = expected.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
		foreach (var expectedProperty in expectedProperties)
		{
			var actualProperty = actual!.GetType().GetProperty(expectedProperty.Name, BindingFlags.Public | BindingFlags.Instance);
			if (actualProperty is null)
			{
				throw new ShouldAssertException(
					new ExpectedEquivalenceShouldlyMessage(expected, actual, [expectedProperty.Name],
							"Expected property does not exist")
						.ToString());
			}

			var expectedValue = expectedProperty.GetValue(expected);
			var actualValue = actualProperty.GetValue(actual);

			try
			{
				actualValue.ShouldBeLike(expectedValue);
			}
			catch (ShouldAssertException ex)
			{
				throw new ShouldAssertException(
					new ExpectedEquivalenceShouldlyMessage(expected, actual, [expectedProperty.Name],
							$"Property '{expectedProperty.Name}' mismatch: {ex.Message}")
						.ToString());
			}
		}
	}

	private static bool IsSimpleType(Type type)
	{
		return type.IsPrimitive
		|| type.IsEnum
		|| type == typeof(string)
		|| type == typeof(decimal)
		|| type == typeof(DateTime)
		|| type == typeof(Guid);
	}
}
