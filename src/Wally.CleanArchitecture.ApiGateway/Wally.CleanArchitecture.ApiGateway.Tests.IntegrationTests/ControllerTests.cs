using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc.Testing;

using Wally.CleanArchitecture.ApiGateway.Tests.IntegrationTests.Helpers;
using Wally.CleanArchitecture.ApiGateway.WebApi;

using Xunit;

namespace Wally.CleanArchitecture.ApiGateway.Tests.IntegrationTests;

public class ControllerTests : IClassFixture<ApiWebApplicationFactory<Startup>>
{
	private readonly ApiWebApplicationFactory<Startup> _factory;

	private readonly HttpClient _httpClient;

	public ControllerTests(ApiWebApplicationFactory<Startup> factory)
	{
		_factory = factory;
		_httpClient = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false, });
	}

	[Fact]
	public async Task Get_RootPath_ReturnsAppVersion()
	{
		// Arrange

		// Act
		var response = await _httpClient.GetAsync($"/");

		// Assert
		response.IsSuccessStatusCode.Should()
			.BeTrue();
		response.StatusCode.Should()
			.Be(HttpStatusCode.OK);
		var content = await response.Content.ReadAsStringAsync();
		content.Should().Be("v1.0.0.0");
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
