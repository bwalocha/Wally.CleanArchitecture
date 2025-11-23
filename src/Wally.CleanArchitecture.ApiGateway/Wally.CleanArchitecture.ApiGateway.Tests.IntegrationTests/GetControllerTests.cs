namespace Wally.CleanArchitecture.ApiGateway.Tests.IntegrationTests;

public partial class ControllerTests
{
	[Fact]
	public async Task Get_RootPath_ReturnsAppVersion()
	{
		// Arrange

		// Act
		var response = await _httpClient.GetAsync("/");

		// Assert
		await Verifier.Verify(response).IgnoreMembers("Content-Length", "version");
	}

	[Fact]
	public async Task Get_NoExistingResource_Returns404()
	{
		// Arrange
		var resourceId = Guid.NewGuid();

		// Act
		var response = await _httpClient.GetAsync($"/{resourceId}");

		// Assert
		await Verifier.Verify(response);
	}
}
