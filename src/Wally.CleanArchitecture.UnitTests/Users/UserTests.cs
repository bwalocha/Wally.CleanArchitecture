using System;
using System.Linq;

using FluentAssertions;

using Wally.CleanArchitecture.Domain.Users;

using Xunit;

namespace Wally.CleanArchitecture.UnitTests.Users;

public class UserTests
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
		user.Id.Should()
			.Be(id);
		user.Name.Should()
			.Be("newTestName");
	}

	[Fact]
	public void Create_ForNewDomainModel_ProducesDomainEvent()
	{
		// Arrange
		var id = Guid.NewGuid();

		// Act
		var model = User.Create(id, "testUserName");

		// Assert
		model.GetDomainEvents()
			.Single()
			.Should()
			.BeOfType<UserCreatedDomainEvent>()
			.Subject.Id.Should()
			.Be(id);
	}
}
