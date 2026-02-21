using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Wally.CleanArchitecture.MicroService.Tests.IntegrationTests.Helpers;
using Wally.CleanArchitecture.MicroService.WebApi;

namespace Wally.CleanArchitecture.MicroService.Tests.IntegrationTests;

[Collection(nameof(IntegrationTests))]
public class OpenApiTests : IDisposable
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

		// Act
		var response = await _httpClient.GetAsync(new Uri("swagger/v1/swagger.json", UriKind.Relative));

		// Assert
		await Verifier.Verify(response).IgnoreMembers("Content-Length");
	}
}
