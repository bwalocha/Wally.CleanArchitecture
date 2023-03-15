using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;

using JsonNet.ContractResolvers;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

using Wally.CleanArchitecture.MicroService.Application.Contracts.Requests.Users;
using Wally.CleanArchitecture.MicroService.Application.Contracts.Responses.Users;
using Wally.CleanArchitecture.MicroService.Domain.Users;
using Wally.CleanArchitecture.MicroService.Tests.IntegrationTests.Helpers;
using Wally.CleanArchitecture.MicroService.WebApi;
using Wally.Lib.DDD.Abstractions.Responses;

using Xunit;

namespace Wally.CleanArchitecture.MicroService.Tests.IntegrationTests;

// [Collection(nameof(UsersControllerTests))]
// [TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
// [Trait("TestTraitNames.Category", "TestCategoryNames.Integration")]
public class UsersControllerTests : IClassFixture<ApiWebApplicationFactory<Startup>>
{
	private readonly DbContext _database;
	private readonly ApiWebApplicationFactory<Startup> _factory;

	private readonly HttpClient _httpClient;
	private readonly JsonSerializerSettings _jsonSettings;

	public UsersControllerTests(ApiWebApplicationFactory<Startup> factory)
	{
		_factory = factory;
		_httpClient = factory.WithWebHostBuilder(
				builder =>
				{
					builder.ConfigureTestServices(
						services =>
						{
							// services.AddAuthentication("Test")
							// 	.AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
						});
				})
			.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false, });
		_jsonSettings = new JsonSerializerSettings { ContractResolver = new PrivateSetterContractResolver(), };

		_database = factory.GetRequiredService<DbContext>();
		_database.RemoveRange(_database.Set<User>());
		_database.SaveChanges();
	}

	[Fact]
	public async Task Get_NoExistingResource_Returns404()
	{
		// Arrange
		var resourceId = Guid.NewGuid();

		// Act
		var response = await _httpClient.GetAsync($"Users/{resourceId}");

		// Assert
		response.IsSuccessStatusCode.Should()
			.BeFalse();
		response.StatusCode.Should()
			.Be(HttpStatusCode.NotFound);
	}

	[Fact]
	public async Task GetOData_NoQueryStringNoResources_ReturnsEmptyResponse()
	{
		// Arrange

		// Act
		var response = await _httpClient.GetAsync("Users");

		// Assert
		response.IsSuccessStatusCode.Should()
			.BeTrue();
		response.StatusCode.Should()
			.Be(HttpStatusCode.OK);
		var data = await response.ReadAsync<PagedResponse<GetUsersResponse>>(CancellationToken.None);
		data.Should()
			.NotBeNull();
		data!.Items.Length.Should()
			.Be(0);
	}

	[Fact]
	public async Task GetOData_Top1NoResources_ReturnsEmptyResponse()
	{
		// Arrange

		// Act
		var response = await _httpClient.GetAsync("Users?$top=1");

		// Assert
		response.IsSuccessStatusCode.Should()
			.BeTrue();
		response.StatusCode.Should()
			.Be(HttpStatusCode.OK);
		var data = await response.ReadAsync<PagedResponse<GetUsersResponse>>(CancellationToken.None);
		data.Should()
			.NotBeNull();
		data!.Items.Length.Should()
			.Be(0);
	}

	[Fact(Skip = "OData Select should not be supported - use Mapping")]
	public async Task GetOData_SelectNameNoUsers_ReturnsEmptyResponse()
	{
		// Arrange
		_database.Add(User.Create("testUser1"));
		_database.Add(User.Create("testUser3"));
		_database.Add(User.Create("testUser2"));
		await _database.SaveChangesAsync();

		// Act
		var response = await _httpClient.GetAsync("Users?$select=name");

		// Assert
		response.IsSuccessStatusCode.Should()
			.BeTrue();
		response.StatusCode.Should()
			.Be(HttpStatusCode.OK);
		var data = await response.ReadAsync<PagedResponse<GetUsersResponse>>(CancellationToken.None);
		data.Should()
			.NotBeNull();
		data!.Items.Length.Should()
			.Be(0);
	}

	[Fact]
	public async Task GetOData_3Resources_Returns3Resources()
	{
		// Arrange
		_database.Add(User.Create("testUser1"));
		_database.Add(User.Create("testUser3"));
		_database.Add(User.Create("testUser2"));
		await _database.SaveChangesAsync();

		// Act
		var response = await _httpClient.GetAsync("Users"); // x3

		// Assert
		response.IsSuccessStatusCode.Should()
			.BeTrue();
		response.StatusCode.Should()
			.Be(HttpStatusCode.OK);
		var data = await response.ReadAsync<PagedResponse<GetUsersResponse>>(CancellationToken.None);
		data.Should()
			.NotBeNull();
		data!.Items.Length.Should()
			.Be(3);
	}

	[Fact]
	public async Task GetOData_3ResourcesOrdered_Returns3Resources()
	{
		// Arrange
		_database.Add(User.Create("testUser1"));
		_database.Add(User.Create("testUser3"));
		_database.Add(User.Create("testUser2"));
		await _database.SaveChangesAsync();

		// Act
		var response = await _httpClient.GetAsync("Users?$orderby=Name"); // x3

		// Assert
		response.IsSuccessStatusCode.Should()
			.BeTrue();
		response.StatusCode.Should()
			.Be(HttpStatusCode.OK);
		var data = await response.ReadAsync<PagedResponse<GetUsersResponse>>(CancellationToken.None);
		data.Should()
			.NotBeNull();
		data!.Items.Length.Should()
			.Be(3);
		data.Items[0]
			.Name.Should()
			.Be("testUser1");
		data.Items[1]
			.Name.Should()
			.Be("testUser2");
		data.Items[2]
			.Name.Should()
			.Be("testUser3");
	}

	[Fact]
	public async Task GetOData_3ResourcesOrderedDesc_Returns3Resources()
	{
		// Arrange
		_database.Add(User.Create("testUser1"));
		_database.Add(User.Create("testUser3"));
		_database.Add(User.Create("testUser2"));
		await _database.SaveChangesAsync();

		// Act
		var response = await _httpClient.GetAsync("Users?$orderby=Name desc"); // x3

		// Assert
		response.IsSuccessStatusCode.Should()
			.BeTrue();
		response.StatusCode.Should()
			.Be(HttpStatusCode.OK);
		var data = await response.ReadAsync<PagedResponse<GetUsersResponse>>(CancellationToken.None);
		data.Should()
			.NotBeNull();
		data!.Items.Length.Should()
			.Be(3);
		data.Items[0]
			.Name.Should()
			.Be("testUser3");
		data.Items[1]
			.Name.Should()
			.Be("testUser2");
		data.Items[2]
			.Name.Should()
			.Be("testUser1");
	}

	[Fact]
	public async Task GetOData_3ResourcesOrderedSkipped_Returns2Resources()
	{
		// Arrange
		_database.Add(User.Create("testUser1"));
		_database.Add(User.Create("testUser3"));
		_database.Add(User.Create("testUser2"));
		await _database.SaveChangesAsync();

		// Act
		var response = await _httpClient.GetAsync("Users?$orderby=Name&$skip=1"); // x2

		// Assert
		response.IsSuccessStatusCode.Should()
			.BeTrue();
		response.StatusCode.Should()
			.Be(HttpStatusCode.OK);
		var data = await response.ReadAsync<PagedResponse<GetUsersResponse>>(CancellationToken.None);
		data.Should()
			.NotBeNull();
		data!.Items.Length.Should()
			.Be(2);
		data.Items[0]
			.Name.Should()
			.Be("testUser2");
		data.Items[1]
			.Name.Should()
			.Be("testUser3");
	}

	[Fact]
	public async Task GetOData_3ResourcesOrderedTop2_Returns2Resources()
	{
		// Arrange
		_database.Add(User.Create("testUser1"));
		_database.Add(User.Create("testUser3"));
		_database.Add(User.Create("testUser2"));
		await _database.SaveChangesAsync();

		// Act
		var response = await _httpClient.GetAsync("Users?$orderby=Name&$top=2"); // x2

		// Assert
		response.IsSuccessStatusCode.Should()
			.BeTrue();
		response.StatusCode.Should()
			.Be(HttpStatusCode.OK);
		var data = await response.ReadAsync<PagedResponse<GetUsersResponse>>(CancellationToken.None);
		data.Should()
			.NotBeNull();
		data!.Items.Length.Should()
			.Be(2);
		data.Items[0]
			.Name.Should()
			.Be("testUser1");
		data.Items[1]
			.Name.Should()
			.Be("testUser2");
	}

	[Fact]
	public async Task GetOData_3ResourcesOrderedSkipped1Top2_Returns2Resources()
	{
		// Arrange
		_database.Add(User.Create("testUser1"));
		_database.Add(User.Create("testUser3"));
		_database.Add(User.Create("testUser2"));
		await _database.SaveChangesAsync();

		// Act
		var response = await _httpClient.GetAsync("Users?$orderby=Name&$skip=1&$top=2"); // 1

		// Assert
		response.IsSuccessStatusCode.Should()
			.BeTrue();
		response.StatusCode.Should()
			.Be(HttpStatusCode.OK);
		var data = await response.ReadAsync<PagedResponse<GetUsersResponse>>(CancellationToken.None);
		data.Should()
			.NotBeNull();
		data!.Items.Length.Should()
			.Be(2);
		data.Items[0]
			.Name.Should()
			.Be("testUser2");
		data.Items[1]
			.Name.Should()
			.Be("testUser3");
	}

	[Fact]
	public async Task GetOData_3ResourcesOrderedSkipped1Top2Filtered_Returns1Resource()
	{
		// Arrange
		_database.Add(User.Create("testUser1"));
		_database.Add(User.Create("testUser3"));
		_database.Add(User.Create("testUser2"));
		await _database.SaveChangesAsync();

		// Act
		var response =
			await _httpClient.GetAsync("Users?$orderby=Name&$skip=1&$top=2&$filter=Name ne 'testUser3'"); // x1

		// Assert
		response.IsSuccessStatusCode.Should()
			.BeTrue();
		response.StatusCode.Should()
			.Be(HttpStatusCode.OK);
		var data = await response.ReadAsync<PagedResponse<GetUsersResponse>>(CancellationToken.None);
		data.Should()
			.NotBeNull();
		data!.Items.Length.Should()
			.Be(1);
		data.Items.Single()
			.Name.Should()
			.Be("testUser2");
	}

	[Fact]
	public async Task Put_ForExistingResource_UpdatesResourceData()
	{
		// Arrange
		var resource = User.Create("testUser3");
		_database.Add(resource);
		await _database.SaveChangesAsync();
		var request = new UpdateUserRequest("newTestResource1");

		// Act
		var response = await _httpClient.PutAsync($"Users/{resource.Id}", request, CancellationToken.None);

		// Assert
		response.IsSuccessStatusCode.Should()
			.BeTrue();
		response.StatusCode.Should()
			.Be(HttpStatusCode.OK);
		_factory.GetRequiredService<DbContext>()
			.Set<User>()
			.Single(a => a.Id == resource.Id)
			.Name.Should()
			.Be("newTestResource1");
	}

	[Fact]
	public async Task Create_ForNewResource_CreatesNewResource()
	{
		// Arrange
		var request = new CreateUserRequest("newName3");

		// Act
		var response = await _httpClient.PostAsync("Users", request, CancellationToken.None);

		// Assert
		response.IsSuccessStatusCode.Should()
			.BeTrue();
		response.StatusCode.Should()
			.Be(HttpStatusCode.OK);
		_factory.GetRequiredService<DbContext>()
			.Set<User>()
			.Single()
			.Name.Should()
			.Be("newName3");
	}

	[Fact]
	public async Task Create_ForInvalidRequest_ReturnsBadRequest()
	{
		// Arrange
		var request = new CreateUserRequest(string.Empty);

		// Act
		var response = await _httpClient.PostAsync("Users", request, CancellationToken.None);

		// Assert
		response.IsSuccessStatusCode.Should()
			.BeFalse();
		response.StatusCode.Should()
			.Be(HttpStatusCode.BadRequest);
		_factory.GetRequiredService<DbContext>()
			.Set<User>()
			.Should()
			.BeEmpty();
	}
}
