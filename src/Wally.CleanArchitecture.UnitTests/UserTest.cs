using System;

using FluentAssertions;

using Wally.CleanArchitecture.Domain.Users;

using Xunit;

namespace Wally.CleanArchitecture.UnitTests;

public class UserTest
{
	[Fact]
	public void Create_WithSpecifiedUserName_SetsIdAndName()
	{
		// Arrange
		User user;
		
		// Act
		user = User.Create("testUserName");

		// Assert
		user.Id.Should()
			.NotBeEmpty();
		user.Name.Should()
			.NotBeNullOrWhiteSpace();
	}
	
	[Fact]
	public void Update_ForSpecifiedUser_UpdatesName()
	{
		// Arrange
		var id = Guid.NewGuid();
		var user = User.Create(id, "testUserName");
		
		// Act
		user.Update("newTestName");

		// Assert
		user.Id.Should().Be(id);
		user.Name.Should().Be("newTestName");
	}
}
