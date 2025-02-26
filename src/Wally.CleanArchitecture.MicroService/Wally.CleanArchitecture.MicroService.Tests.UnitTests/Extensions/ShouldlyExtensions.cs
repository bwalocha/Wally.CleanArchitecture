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
				actualValue.ShouldBe(expectedValue, $"Property '{expectedProperty.Name}' does not match.");
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
}
