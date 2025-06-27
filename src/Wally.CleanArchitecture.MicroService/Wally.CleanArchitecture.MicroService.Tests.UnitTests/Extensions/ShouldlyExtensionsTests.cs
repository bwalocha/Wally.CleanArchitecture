using System.Collections.Generic;

namespace Wally.CleanArchitecture.MicroService.Tests.UnitTests.Extensions;

public class ShouldlyExtensionsTests
{
	[Theory]
	[MemberData(nameof(GetValidTestData))]
	public void ShouldBeLike_ForTwoObjects_ShouldNotThrow(object actual, object expected)
	{
		// Arrange

		// Act
		var act = () => actual.ShouldBeLike(expected);

		// Act & Assert
		act.ShouldNotThrow();
	}
	
	[Theory]
	[MemberData(nameof(GetInvalidTestData))]
	public void ShouldBeLike_ForTwoDifferentObjects_ShouldThrow(object actual, object expected)
	{
		// Arrange

		// Act
		var act = () => actual.ShouldBeLike(expected);

		// Act & Assert
		act.ShouldThrow<ShouldAssertException>();
	}

	public static IEnumerable<object?[]> GetValidTestData()
	{
		yield return [0, 0];
		
		yield return [0L, 0d];
		
		yield return [1.2, 1.2];
		
		yield return [new [] {1, 2d, 3}, new [] {1, 2M, 3}];

		yield return ["test", "test"];
		
		yield return [new { Id = 1, Name = "testName" }, new { Name = "testName" }];
		
		yield return [new { Id = 1, Name = "testName", Nested = new { Id = 8 } }, new { Name = "testName" }];
		
		yield return [new { Id = 1, Name = "testName", Nested = new { Id = 8 } }, new { Name = "testName", Nested = new { Id = 8 } }];
		
		yield return [new { Id = 1, Name = "testName", Nested = new { Id = 8, Items = new [] {1, 2, 3} } }, new { Name = "testName", Nested = new { Id = 8 } }];
	}
	
	public static IEnumerable<object?[]> GetInvalidTestData()
	{
		yield return [0, 0.1];
		
		yield return [1.2, 1.21];
		
		yield return [new [] {1, 2, 3}, new [] {1, 2, 3, 4}];
		
		yield return ["test", "Test"];
		
		yield return [new { Name = "testName" }, new { Name = "testInvalidName" }];
		
		yield return [new { Name = "testName" }, new { Id = 1, Name = "testName" }];
		
		yield return [new { Id = 1, Name = "testName", Nested = new { Id = 8 } }, new { Name = "test" }];
		
		yield return [new { Id = 1, Name = "testName", Nested = new { Id = 8.1 } }, new { Name = "testName", Nested = new { Id = 8 } }];
		
		yield return [new { Id = 1, Name = "testName", Nested = new { Id = 8, Items = new [] {1, 2, 3} } }, new { Name = "testName", Nested = new { Id = 8, Items = "1, 2, 3" } }];
	}
}
