using Wally.CleanArchitecture.MicroService.Tests.IntegrationTests.Extensions;
using Wally.CleanArchitecture.MicroService.WebApi.Contracts.Users.Requests;

namespace Wally.CleanArchitecture.MicroService.Tests.IntegrationTests;

public partial class UsersControllerTests
{
	[Fact]
	public async Task Put_ForNonExistingResource_ReturnsResourceNotFound()
	{
		// Arrange
		var resource = UserCreate(3);
		var request = new UpdateUserRequest
		{
			Name = "newTestResource1",
		};

		// Act
		var response = await _httpClient.PutAsync($"Users/{resource.Id.Value}", request, CancellationToken.None);

		// Assert
		await Verifier.Verify(response);
	}

	[Fact]
	public async Task Put_ForInvalidRequest_ReturnsBadRequest()
	{
		// Arrange
		var resource = UserCreate(3);
		var request = new UpdateUserRequest
		{
			Name = string.Empty,
		};

		// Act
		var response = await _httpClient.PutAsync($"Users/{resource.Id.Value}", request, CancellationToken.None);

		// Assert
		await Verifier.Verify(response);
	}

	[Fact]
	public async Task Put_ForExistingResource_UpdatesResourceData()
	{
		// Arrange
		var resource = UserCreate(3);
		await _factory.SeedAsync(resource);
		var request = new UpdateUserRequest
		{
			Name = "newTestResource1",
		};

		// Act
		var response = await _httpClient.PutAsync($"Users/{resource.Id.Value}", request, CancellationToken.None);

		// Assert
		await Verifier.Verify(response);
	}
}
