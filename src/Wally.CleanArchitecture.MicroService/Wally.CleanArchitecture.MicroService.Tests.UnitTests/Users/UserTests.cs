using System;
using System.Linq;
using Shouldly;
using Wally.CleanArchitecture.MicroService.Domain.Users;
using Xunit;

namespace Wally.CleanArchitecture.MicroService.Tests.UnitTests.Users;

public class UserTests
{
	[Fact]
	public void Create_WithSpecifiedUserName_SetsIdAndName()
	{
		// Arrange
		User sut;

		// Act
		sut = User.Create("testUserName");

		// Assert
		sut.ShouldSatisfyAllConditions(
			() => sut.Id.ShouldNotBeNull(),
			// () => user.Id.Value.ShouldNotBeNullOrEmpty(), // TODO: Guid does not have isEmpty/notEmpty assertion
			() => sut.Id.Value.ShouldNotBe(Guid.Empty),
			() => sut.Name.ShouldNotBeNullOrWhiteSpace());
	}

	[Fact]
	public void Create_ForNewDomainModel_ProducesDomainEvent()
	{
		// Arrange
		var id = new UserId();

		// Act
		var sut = User.Create(id, "testUserName");

		// Assert
		sut.ShouldSatisfyAllConditions(
			() => sut.Id.ShouldBe(id),
			() => sut.GetDomainEvents()
				.Single()
				.ShouldBeOfType<UserCreatedDomainEvent>());
	}
	
	[Fact]
	public void Update_ForSpecifiedUser_UpdatesName()
	{
		// Arrange
		var id = new UserId();
		var sut = User.Create(id, "testUserName");

		// Act
		sut.Update("newTestName");

		// Assert
		sut.ShouldSatisfyAllConditions(
			() => sut.Id.ShouldBe(id),
			() => sut.Name.ShouldBe("newTestName"));
	}
}
