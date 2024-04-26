using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Wally.CleanArchitecture.MicroService.Application.Contracts.Requests.Users;
using Wally.CleanArchitecture.MicroService.Application.Contracts.Responses.Users;
using Wally.CleanArchitecture.MicroService.Domain.Users;
using Wally.CleanArchitecture.MicroService.Tests.IntegrationTests.Extensions;
using Wally.CleanArchitecture.MicroService.Tests.IntegrationTests.Helpers;
using Wally.CleanArchitecture.MicroService.WebApi;
using Wally.Lib.DDD.Abstractions.Responses;
using Xunit;

namespace Wally.CleanArchitecture.MicroService.Tests.IntegrationTests;

public class UsersControllerTests : IClassFixture<ApiWebApplicationFactory<Startup>>
{
	private readonly DbContext _database;
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
		_database = factory.GetRequiredService<DbContext>();
		_database.RemoveRange(_database.Set<User>());
		_database.SaveChanges();
	}

	private static User UserCreate(int index)
	{
		var resource = User.Create($"testUser{index}");
		var createdByIdProperty = typeof(User).GetProperty(nameof(User.CreatedById)) !;
		createdByIdProperty.DeclaringType!.GetProperty(nameof(User.CreatedById)) !.SetValue(
			resource,
			new UserId(Guid.NewGuid()));

		return resource;
	}

	[Fact]
	public async Task Get_NoExistingResource_Returns404()
	{
		// Arrange
		var resourceId = Guid.NewGuid();

		// Act
		var response = await _httpClient.GetAsync(new Uri($"Users/{resourceId}", UriKind.Relative));

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
		var response = await _httpClient.GetAsync(new Uri("Users", UriKind.Relative));

		// Assert
		response.IsSuccessStatusCode.Should()
			.BeTrue();
		response.StatusCode.Should()
			.Be(HttpStatusCode.OK);
		var data = await response.ReadAsync<PagedResponse<GetUsersResponse>>(CancellationToken.None);
		data.Should()
			.NotBeNull();
		data.Items.Length.Should()
			.Be(0);
	}

	[Fact]
	public async Task GetOData_Top1NoResources_ReturnsEmptyResponse()
	{
		// Arrange

		// Act
		var response = await _httpClient.GetAsync(new Uri("Users?$top=1", UriKind.Relative));

		// Assert
		response.IsSuccessStatusCode.Should()
			.BeTrue();
		response.StatusCode.Should()
			.Be(HttpStatusCode.OK);
		var data = await response.ReadAsync<PagedResponse<GetUsersResponse>>(CancellationToken.None);
		data.Should()
			.NotBeNull();
		data.Items.Length.Should()
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
		var response = await _httpClient.GetAsync(new Uri("Users?$select=name", UriKind.Relative));

		// Assert
		response.IsSuccessStatusCode.Should()
			.BeTrue();
		response.StatusCode.Should()
			.Be(HttpStatusCode.OK);
		var data = await response.ReadAsync<PagedResponse<GetUsersResponse>>(CancellationToken.None);
		data.Should()
			.NotBeNull();
		data.Items.Length.Should()
			.Be(0);
	}

	[Fact]
	public async Task GetOData_3Resources_Returns3Resources()
	{
		// Arrange
		_database.Add(UserCreate(1));
		_database.Add(UserCreate(3));
		_database.Add(UserCreate(2));
		await _database.SaveChangesAsync();

		// Act
		var response = await _httpClient.GetAsync(new Uri("Users", UriKind.Relative)); // x3

		// Assert
		response.IsSuccessStatusCode.Should()
			.BeTrue();
		response.StatusCode.Should()
			.Be(HttpStatusCode.OK);
		var data = await response.ReadAsync<PagedResponse<GetUsersResponse>>(CancellationToken.None);
		data.Should()
			.NotBeNull();
		data.Items.Length.Should()
			.Be(3);
	}

	[Fact]
	public async Task GetOData_3ResourcesOrdered_Returns3Resources()
	{
		// Arrange
		_database.Add(UserCreate(1));
		_database.Add(UserCreate(3));
		_database.Add(UserCreate(2));
		await _database.SaveChangesAsync();

		// Act
		var response = await _httpClient.GetAsync(new Uri("Users?$orderby=Name", UriKind.Relative)); // x3

		// Assert
		response.IsSuccessStatusCode.Should()
			.BeTrue();
		response.StatusCode.Should()
			.Be(HttpStatusCode.OK);
		var data = await response.ReadAsync<PagedResponse<GetUsersResponse>>(CancellationToken.None);
		data.Should()
			.NotBeNull();
		data.Items.Length.Should()
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
		_database.Add(UserCreate(1));
		_database.Add(UserCreate(3));
		_database.Add(UserCreate(2));
		await _database.SaveChangesAsync();

		// Act
		var response = await _httpClient.GetAsync(new Uri("Users?$orderby=Name desc", UriKind.Relative)); // x3

		// Assert
		response.IsSuccessStatusCode.Should()
			.BeTrue();
		response.StatusCode.Should()
			.Be(HttpStatusCode.OK);
		var data = await response.ReadAsync<PagedResponse<GetUsersResponse>>(CancellationToken.None);
		data.Should()
			.NotBeNull();
		data.Items.Length.Should()
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

	[Fact(Skip = "The User Name is Unique and the test cannot be performed")]
	public async Task GetOData_3ResourcesOrderedBy2Properties_Returns3Resources()
	{
		// Arrange
		_database.Add(UserCreate(3));
		_database.Add(UserCreate(3));
		_database.Add(UserCreate(2));
		await _database.SaveChangesAsync();

		// Act
		var response = await _httpClient.GetAsync(new Uri("Users?$orderby=Name asc, Id desc", UriKind.Relative)); // x3

		// Assert
		response.IsSuccessStatusCode.Should()
			.BeTrue();
		response.StatusCode.Should()
			.Be(HttpStatusCode.OK);
		var data = await response.ReadAsync<PagedResponse<GetUsersResponse>>(CancellationToken.None);
		data.Should()
			.NotBeNull();
		data.Items.Length.Should()
			.Be(3);
		data.Items[0]
			.Name.Should()
			.Be("testUser2");
		data.Items[1]
			.Name.Should()
			.Be("testUser3");
		data.Items[2]
			.Name.Should()
			.Be("testUser3");
		data.Items[1]
			.Id.CompareTo(data.Items[2].Id)
			.Should()
			.Be(1);
	}

	[Fact]
	public async Task GetOData_3ResourcesOrderedSkipped_Returns2Resources()
	{
		// Arrange
		_database.Add(UserCreate(1));
		_database.Add(UserCreate(3));
		_database.Add(UserCreate(2));
		await _database.SaveChangesAsync();

		// Act
		var response = await _httpClient.GetAsync(new Uri("Users?$orderby=Name&$skip=1", UriKind.Relative)); // x2

		// Assert
		response.IsSuccessStatusCode.Should()
			.BeTrue();
		response.StatusCode.Should()
			.Be(HttpStatusCode.OK);
		var data = await response.ReadAsync<PagedResponse<GetUsersResponse>>(CancellationToken.None);
		data.Should()
			.NotBeNull();
		data.Items.Length.Should()
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
		_database.Add(UserCreate(1));
		_database.Add(UserCreate(3));
		_database.Add(UserCreate(2));
		await _database.SaveChangesAsync();

		// Act
		var response = await _httpClient.GetAsync(new Uri("Users?$orderby=Name&$top=2", UriKind.Relative)); // x2

		// Assert
		response.IsSuccessStatusCode.Should()
			.BeTrue();
		response.StatusCode.Should()
			.Be(HttpStatusCode.OK);
		var data = await response.ReadAsync<PagedResponse<GetUsersResponse>>(CancellationToken.None);
		data.Should()
			.NotBeNull();
		data.Items.Length.Should()
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
		_database.Add(UserCreate(1));
		_database.Add(UserCreate(3));
		_database.Add(UserCreate(2));
		await _database.SaveChangesAsync();

		// Act
		var response = await _httpClient.GetAsync(new Uri("Users?$orderby=Name&$skip=1&$top=2", UriKind.Relative)); // 1

		// Assert
		response.IsSuccessStatusCode.Should()
			.BeTrue();
		response.StatusCode.Should()
			.Be(HttpStatusCode.OK);
		var data = await response.ReadAsync<PagedResponse<GetUsersResponse>>(CancellationToken.None);
		data.Should()
			.NotBeNull();
		data.Items.Length.Should()
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
		_database.Add(UserCreate(1));
		_database.Add(UserCreate(3));
		_database.Add(UserCreate(2));
		await _database.SaveChangesAsync();

		// Act
		var response =
			await _httpClient.GetAsync(new Uri("Users?$orderby=Name&$skip=1&$top=2&$filter=Name ne 'testUser3'",
				UriKind.Relative)); // x1

		// Assert
		response.IsSuccessStatusCode.Should()
			.BeTrue();
		response.StatusCode.Should()
			.Be(HttpStatusCode.OK);
		var data = await response.ReadAsync<PagedResponse<GetUsersResponse>>(CancellationToken.None);
		data.Should()
			.NotBeNull();
		data.Items.Length.Should()
			.Be(1);
		data.Items.Single()
			.Name.Should()
			.Be("testUser2");
	}

	[Fact]
	public async Task Put_ForExistingResource_UpdatesResourceData()
	{
		// Arrange
		var resource = UserCreate(3);
		_database.Add(resource);
		await _database.SaveChangesAsync();
		var request = new UpdateUserRequest("newTestResource1");

		// Act
		var response = await _httpClient.PutAsync($"Users/{resource.Id.Value}", request, CancellationToken.None);

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

	[Fact]
	public async Task Delete_ForExistingResource_SoftDeletesResourceData()
	{
		// Arrange
		var resource1 = UserCreate(1);
		var resource2 = UserCreate(2);
		_database.Add(resource1);
		_database.Add(resource2);
		await _database.SaveChangesAsync();

		// Act
		var response = await _httpClient.DeleteAsync($"Users/{resource2.Id.Value}", CancellationToken.None);

		// Assert
		response.IsSuccessStatusCode.Should()
			.BeTrue();
		response.StatusCode.Should()
			.Be(HttpStatusCode.Accepted);
		_factory.GetRequiredService<DbContext>()
			.Set<User>()
			.Single(a => a.Id == resource1.Id)
			.IsDeleted.Should()
			.Be(false);
		_factory.GetRequiredService<DbContext>()
			.Set<User>()
			.FirstOrDefault(a => a.Id == resource2.Id)
			.Should()
			.BeNull();
		_factory.GetRequiredService<DbContext>()
			.Set<User>()
			.IgnoreQueryFilters()
			.Single(a => a.Id == resource1.Id)
			.IsDeleted.Should()
			.Be(false);
		_factory.GetRequiredService<DbContext>()
			.Set<User>()
			.IgnoreQueryFilters()
			.Single(a => a.Id == resource2.Id)
			.IsDeleted.Should()
			.Be(true);
	}
}
