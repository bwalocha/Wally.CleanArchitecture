using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Wally.CleanArchitecture.MicroService.Tests.IntegrationTests.Extensions;
using Wally.CleanArchitecture.MicroService.Tests.IntegrationTests.Helpers;
using Wally.CleanArchitecture.MicroService.WebApi;
using Xunit;

namespace Wally.CleanArchitecture.MicroService.Tests.IntegrationTests;

public class OpenApiTests : IClassFixture<ApiWebApplicationFactory<Startup>>, IDisposable
{
	private readonly HttpClient _httpClient;

	public OpenApiTests(ApiWebApplicationFactory<Startup> factory)
	{
		_httpClient = factory.CreateClient(
			new WebApplicationFactoryClientOptions
			{
				AllowAutoRedirect = false,
			});
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposing)
		{
			_httpClient.Dispose();
		}
	}

	[Fact]
	public async Task Get_OpenApi_ReturnsApiSpecification()
	{
		// Arrange
		var expectedJson = await System.IO.File.ReadAllTextAsync("Resources/openapi.json");
		var expectedResponseObject = JsonConvert.DeserializeObject<dynamic>(expectedJson);

		// Act
		var response = await _httpClient.GetAsync(new Uri("swagger/v1/swagger.json", UriKind.Relative));

		// Assert
		response.IsSuccessStatusCode.Should()
			.BeTrue();
		response.StatusCode.Should()
			.Be(HttpStatusCode.OK);

		var data = await response.Content.ReadAsStringAsync(CancellationToken.None);
		data.Should()
			.NotBeNull();

		var actualResponseObject = JsonConvert.DeserializeObject<dynamic>(data);
		Assert.Equal(expectedResponseObject, actualResponseObject);
	}
}
