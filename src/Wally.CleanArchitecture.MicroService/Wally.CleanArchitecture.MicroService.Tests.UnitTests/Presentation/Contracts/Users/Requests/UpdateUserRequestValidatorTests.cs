using FluentValidation;
using Wally.CleanArchitecture.MicroService.WebApi.Contracts.Users.Requests;

namespace Wally.CleanArchitecture.MicroService.Tests.UnitTests.Presentation.Contracts.Users.Requests;

// https://docs.fluentvalidation.net/en/latest/testing.html
public class UpdateUserRequestValidatorTests
{
	private readonly AbstractValidator<UpdateUserRequest> _sut;

	public UpdateUserRequestValidatorTests()
	{
		_sut = new UpdateUserRequestValidator();
	}

	[Theory]
	[InlineData("a")]
	[InlineData("testName")]
	[InlineData(
		"testName Not Loong... ... ... ... ... ... ... ... " +
		"... ... ... ... ... ... ... ... ... ... ... ... .." +
		". ... ... ... ... ... ... ... ... ... ... ... ... " +
		"... ... ... ... ... ... ... ... ... ... ... ... .." +
		". ... ... ... ... ... ... ... ... ... ... ... ... " +
		"... ..")]
	public void Validate_ForValidData_IsValid(string name)
	{
		// Arrange
		var instance = new UpdateUserRequest
		{
			Name = name,
		};

		// Act
		var result = _sut.Validate(instance);

		// Assert
		result.IsValid.ShouldBeTrue();
	}

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData(
		"testName Too Loong... ... ... ... ... ... ... ... " +
		"... ... ... ... ... ... ... ... ... ... ... ... .." +
		". ... ... ... ... ... ... ... ... ... ... ... ... " +
		"... ... ... ... ... ... ... ... ... ... ... ... .." +
		". ... ... ... ... ... ... ... ... ... ... ... ... " +
		"... ...")]
	public void Validate_ForInvalidData_IsNotValid(string? name)
	{
		// Arrange
		var instance = new UpdateUserRequest
		{
			Name = name!,
		};

		// Act
		var result = _sut.Validate(instance);

		// Assert
		result.IsValid.ShouldBeFalse();
	}
}
