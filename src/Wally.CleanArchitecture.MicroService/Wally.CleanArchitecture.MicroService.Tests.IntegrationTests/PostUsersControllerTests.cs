using Wally.CleanArchitecture.MicroService.Tests.IntegrationTests.Extensions;
using Wally.CleanArchitecture.MicroService.WebApi.Contracts.Users.Requests;

namespace Wally.CleanArchitecture.MicroService.Tests.IntegrationTests;

public partial class UsersControllerTests
{
	[Fact]
	public async Task Post_ForInvalidRequest_ReturnsBadRequest()
	{
		// Arrange
		var request = new CreateUserRequest
		{
			Name = string.Empty,
		};

		// Act
		var response = await _httpClient.PostAsync("Users", request, CancellationToken.None);

		// Assert
		await Verifier.Verify(response);
	}

	[Fact]
	public async Task Post_ForNewResource_CreatesNewResource()
	{
		// Arrange
		var request = new CreateUserRequest
		{
			Name = "newName3",
		};

		// Act
		var response = await _httpClient.PostAsync("Users", request, CancellationToken.None);

		// Assert
		await Verifier.Verify(response);
	}

	[Fact]
	public async Task Post_ForDuplicatedResource_ReturnsConflict()
	{
		// Arrange
		await _factory.SeedAsync(UserCreate(3));
		var request = new CreateUserRequest
		{
			Name = "testUser3",
		};

		// Act
		var response = await _httpClient.PostAsync("Users", request, CancellationToken.None);

		// Assert
		await Verifier.Verify(response);
	}
}
