using FluentAssertions;

using FluentValidation;

using Wally.CleanArchitecture.Contracts.Requests.User;

using Xunit;

namespace Wally.CleanArchitecture.UnitTests;

public class GetUsersRequestValidatorTests
{
	private readonly AbstractValidator<GetUsersRequest> _validator;

	public GetUsersRequestValidatorTests()
	{
		_validator = new GetUsersRequestValidator();
	}

	[Fact]
	public void Validate_ForValidData_IsValid()
	{
		// Arrange
		var instance = new GetUsersRequest();

		// Act
		var result = _validator.Validate(instance);

		// Assert
		result.IsValid.Should()
			.BeTrue();
	}
}
