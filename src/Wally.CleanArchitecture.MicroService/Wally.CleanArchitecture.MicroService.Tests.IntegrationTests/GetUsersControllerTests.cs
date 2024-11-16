using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using VerifyXunit;
using Wally.CleanArchitecture.MicroService.Application.Contracts;
using Wally.CleanArchitecture.MicroService.Application.Contracts.Users.Responses;
using Wally.CleanArchitecture.MicroService.Tests.IntegrationTests.Extensions;
using Xunit;

namespace Wally.CleanArchitecture.MicroService.Tests.IntegrationTests;

public partial class UsersControllerTests
{
	[Fact]
	public async Task Get_NoExistingResource_Returns404()
	{
		// Arrange
		var resourceId = Guid.NewGuid();

		// Act
		var response = await _httpClient.GetAsync(new Uri($"Users/{resourceId}", UriKind.Relative));

		// Assert
		await Verifier.Verify(response);
	}

	[Fact]
	public async Task Get_NoQueryStringNoResources_ReturnsEmptyResponse()
	{
		// Arrange

		// Act
		var response = await _httpClient.GetAsync(new Uri("Users", UriKind.Relative));

		// Assert
		await Verifier.Verify(response);

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
	public async Task Get_ExistingResource_ReturnsData()
	{
		// Arrange
		var resource = UserCreate(1);
		await _factory.SeedAsync(resource);

		// Act
		var response = await _httpClient.GetAsync(new Uri($"Users/{resource.Id}", UriKind.Relative));

		// Assert
		await Verifier.Verify(response);

		response.IsSuccessStatusCode.Should()
			.BeTrue();
		response.StatusCode.Should()
			.Be(HttpStatusCode.OK);

		var data = await response.ReadAsync<GetUserResponse>(CancellationToken.None);
		data.Should()
			.BeEquivalentTo(new
			{
				Id = resource.Id.Value,
				resource.Name,
			});
	}

	[Fact]
	public async Task Get_Top1NoResources_ReturnsEmptyResponse()
	{
		// Arrange

		// Act
		var response = await _httpClient.GetAsync(new Uri("Users?$top=1", UriKind.Relative));

		// Assert
		await Verifier.Verify(response);

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
	public async Task Get_SelectNameNoUsers_ReturnsEmptyResponse()
	{
		// Arrange
		await _factory.SeedAsync(
			UserCreate(1),
			UserCreate(3),
			UserCreate(2));

		// Act
		var response = await _httpClient.GetAsync(new Uri("Users?$select=name", UriKind.Relative));

		// Assert
		await Verifier.Verify(response);

		response.IsSuccessStatusCode.Should()
			.BeTrue();
		response.StatusCode.Should()
			.Be(HttpStatusCode.OK);
		var data = await response.ReadAsync<PagedResponse<GetUsersResponse>>(CancellationToken.None);
		data.Should()
			.NotBeNull();
		data.Items.Should()
			.BeEmpty();
	}

	[Fact]
	public async Task Get_3Resources_Returns3Resources()
	{
		// Arrange
		await _factory.SeedAsync(
			UserCreate(1),
			UserCreate(3),
			UserCreate(2));

		// Act
		var response = await _httpClient.GetAsync(new Uri("Users", UriKind.Relative)); // x3

		// Assert
		await Verifier.Verify(response)
			.IgnoreMembers("name");
	}

	[Fact]
	public async Task Get_3ResourcesOrdered_Returns3Resources()
	{
		// Arrange
		await _factory.SeedAsync(
			UserCreate(1),
			UserCreate(3),
			UserCreate(2));

		// Act
		var response = await _httpClient.GetAsync(new Uri("Users?$orderby=Name", UriKind.Relative)); // x3

		// Assert
		await Verifier.Verify(response);
	}

	[Fact]
	public async Task Get_3ResourcesOrderedDesc_Returns3Resources()
	{
		// Arrange
		await _factory.SeedAsync(
			UserCreate(1),
			UserCreate(3),
			UserCreate(2));

		// Act
		var response = await _httpClient.GetAsync(new Uri("Users?$orderby=Name desc", UriKind.Relative)); // x3

		// Assert
		await Verifier.Verify(response);
	}

	[Fact(Skip = "The User Name is Unique and the test cannot be performed")]
	public async Task Get_3ResourcesOrderedBy2Properties_Returns3Resources()
	{
		// Arrange
		await _factory.SeedAsync(
			UserCreate(1),
			UserCreate(3),
			UserCreate(2));

		// Act
		var response = await _httpClient.GetAsync(new Uri("Users?$orderby=Name asc, Id desc", UriKind.Relative)); // x3

		// Assert
		await Verifier.Verify(response);
	}

	[Fact]
	public async Task Get_3ResourcesOrderedSkipped_Returns2Resources()
	{
		// Arrange
		await _factory.SeedAsync(
			UserCreate(1),
			UserCreate(3),
			UserCreate(2));

		// Act
		var response = await _httpClient.GetAsync(new Uri("Users?$orderby=Name&$skip=1", UriKind.Relative)); // x2

		// Assert
		await Verifier.Verify(response);
	}

	[Fact]
	public async Task Get_3ResourcesOrderedTop2_Returns2Resources()
	{
		// Arrange
		await _factory.SeedAsync(
			UserCreate(1),
			UserCreate(3),
			UserCreate(2));

		// Act
		var response = await _httpClient.GetAsync(new Uri("Users?$orderby=Name&$top=2", UriKind.Relative)); // x2

		// Assert
		await Verifier.Verify(response);
	}

	[Fact]
	public async Task Get_3ResourcesOrderedSkipped1Top2_Returns2Resources()
	{
		// Arrange
		await _factory.SeedAsync(
			UserCreate(1),
			UserCreate(3),
			UserCreate(2));

		// Act
		var response = await _httpClient.GetAsync(new Uri("Users?$orderby=Name&$skip=1&$top=2", UriKind.Relative)); // 1

		// Assert
		await Verifier.Verify(response);
	}

	[Fact]
	public async Task Get_3ResourcesOrderedSkipped1Top2Filtered_Returns1Resource()
	{
		// Arrange
		await _factory.SeedAsync(
			UserCreate(1),
			UserCreate(3),
			UserCreate(2));

		// Act
		var response =
			await _httpClient.GetAsync(new Uri("Users?$orderby=Name&$skip=1&$top=2&$filter=Name ne 'testUser3'",
				UriKind.Relative)); // x1

		// Assert
		await Verifier.Verify(response);
	}

	[Fact]
	public async Task Get_3ResourcesOrderedSkipped1Top2FilteredById_Returns1Resource()
	{
		// Arrange
		await _factory.SeedAsync(
			UserCreate(1),
			UserCreate(3),
			UserCreate(2));

		// Act
		var response =
			await _httpClient.GetAsync(new Uri($"Users?$orderby=Name&$skip=1&$top=2&$filter=Id ne ({Guid.NewGuid()})",
				UriKind.Relative)); // x1

		// Assert
		await Verifier.Verify(response);
	}
}
