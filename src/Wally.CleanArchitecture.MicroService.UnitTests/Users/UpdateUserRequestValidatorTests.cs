using FluentAssertions;

using FluentValidation;

using Wally.CleanArchitecture.MicroService.Contracts.Requests.Users;

using Xunit;

namespace Wally.CleanArchitecture.MicroService.UnitTests.Users;

// https://docs.fluentvalidation.net/en/latest/testing.html
public class UpdateUserRequestValidatorTests
{
	private readonly AbstractValidator<UpdateUserRequest> _validator;

	public UpdateUserRequestValidatorTests()
	{
		_validator = new UpdateUserRequestValidator();
	}

	[Theory]
	[InlineData("a")]
	[InlineData("testName")]
	[InlineData(
		"testName Not Loong... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ..")]
	public void Validate_ForValidData_IsValid(string name)
	{
		// Arrange
		var instance = new UpdateUserRequest(name);

		// Act
		var result = _validator.Validate(instance);

		// Assert
		result.IsValid.Should()
			.BeTrue();
	}

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData(
		"testName Too Loong... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ... ...")]
	public void Validate_ForInvalidData_IsNotValid(string name)
	{
		// Arrange
		var instance = new UpdateUserRequest(name);

		// Act
		var result = _validator.Validate(instance);

		// Assert
		result.IsValid.Should()
			.BeFalse();
	}
}
