using System;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Wally.CleanArchitecture.ApiGateway.Tests.IntegrationTests.Helpers;
using Wally.CleanArchitecture.ApiGateway.WebApi;
using Xunit;

namespace Wally.CleanArchitecture.ApiGateway.Tests.IntegrationTests;

public partial class ControllerTests : IClassFixture<ApiWebApplicationFactory<Startup>>, IDisposable
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
}
