using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Wally.CleanArchitecture.MicroService.Domain.Users;
using Wally.CleanArchitecture.MicroService.Tests.IntegrationTests.Extensions;
using Wally.CleanArchitecture.MicroService.Tests.IntegrationTests.Helpers;
using Wally.CleanArchitecture.MicroService.WebApi;

namespace Wally.CleanArchitecture.MicroService.Tests.IntegrationTests;

[Collection(nameof(IntegrationTests))]
public partial class UsersControllerTests : IAsyncLifetime, IDisposable
{
	private readonly ApiWebApplicationFactory<Startup> _factory;

	private readonly HttpClient _httpClient;

	public UsersControllerTests(ApiWebApplicationFactory<Startup> factory)
	{
		_factory = factory;
		_httpClient = factory.CreateClient(
			new WebApplicationFactoryClientOptions
			{
				AllowAutoRedirect = false,
			});
	}

	public Task InitializeAsync()
	{
		// Clean the Database
		return _factory.RemoveAllAsync<User>();
	}

	public Task DisposeAsync()
	{
		return Task.CompletedTask;
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

	private static User UserCreate(int index)
	{
		var userId = new UserId();
		var resource = User
			.Create($"testUser{index}")
			.SetCreatedById(userId);

		return resource;
	}
}
