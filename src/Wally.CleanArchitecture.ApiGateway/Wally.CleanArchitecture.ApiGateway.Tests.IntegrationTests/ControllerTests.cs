using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Wally.CleanArchitecture.ApiGateway.Tests.IntegrationTests.Helpers;
using Wally.CleanArchitecture.ApiGateway.WebApi;
using Xunit;

namespace Wally.CleanArchitecture.ApiGateway.Tests.IntegrationTests;

[SuppressMessage("Major Code Smell", "S4005:\"System.Uri\" arguments should be used instead of strings")]
public class ControllerTests : IClassFixture<ApiWebApplicationFactory<Startup>>
{
	private readonly HttpClient _httpClient;

	public ControllerTests(ApiWebApplicationFactory<Startup> factory)
	{
		_httpClient = factory.CreateClient(
			new WebApplicationFactoryClientOptions
			{
				AllowAutoRedirect = false,
			});
	}

	[Fact]
	public async Task Get_RootPath_ReturnsAppVersion()
	{
		// Arrange

		// Act
		var response = await _httpClient.GetAsync("/");

		// Assert
		response.IsSuccessStatusCode.Should()
			.BeTrue();
		response.StatusCode.Should()
			.Be(HttpStatusCode.OK);
		var content = await response.Content.ReadAsStringAsync();
		content.Should()
			.MatchRegex(@"v\d+\.\d+\.\d+\.\d+");
	}

	[Fact]
	public async Task Get_NoExistingResource_Returns404()
	{
		// Arrange
		var resourceId = Guid.NewGuid();

		// Act
		var response = await _httpClient.GetAsync($"/{resourceId}");

		// Assert
		response.IsSuccessStatusCode.Should()
			.BeFalse();
		response.StatusCode.Should()
			.Be(HttpStatusCode.NotFound);
	}
}
