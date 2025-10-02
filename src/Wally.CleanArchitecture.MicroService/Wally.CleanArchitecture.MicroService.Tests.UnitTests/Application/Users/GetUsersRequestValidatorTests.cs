using FluentValidation;
using Wally.CleanArchitecture.MicroService.Application.Users.Requests;

namespace Wally.CleanArchitecture.MicroService.Tests.UnitTests.Application.Users;

// https://docs.fluentvalidation.net/en/latest/testing.html
public class GetUsersRequestValidatorTests
{
	private readonly AbstractValidator<GetUsersRequest> _sut;

	public GetUsersRequestValidatorTests()
	{
		_sut = new GetUsersRequestValidator();
	}

	[Fact]
	public void Validate_ForValidData_IsValid()
	{
		// Arrange
		var instance = new GetUsersRequest();

		// Act
		var result = _sut.Validate(instance);

		// Assert
		result.IsValid.ShouldBeTrue();
	}
}
