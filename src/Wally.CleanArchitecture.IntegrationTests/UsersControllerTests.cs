using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Wally.CleanArchitecture.Contracts.Responses.Users;
using Wally.CleanArchitecture.Persistence;
using Wally.CleanArchitecture.WebApi;
using Xunit;
using JsonNet.ContractResolvers;
using Wally.CleanArchitecture.Domain.Users;
using Wally.Lib.DDD.Abstractions.Responses;

// [assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace Wally.CleanArchitecture.IntegrationTests
{
	// [Collection(nameof(UsersControllerTests))]
	// [TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
	// [Trait("TestTraitNames.Category", "TestCategoryNames.Integration")]
	public class UsersControllerTests : IClassFixture<ApiWebApplicationFactory<Startup>>
	{
		private readonly ApiWebApplicationFactory<Startup> _factory;

		private readonly HttpClient _httpClient;
		private readonly DbContext _database;
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
				.CreateClient(
					new WebApplicationFactoryClientOptions
					{
						AllowAutoRedirect = false,
					});
			_jsonSettings = new JsonSerializerSettings
			{
				ContractResolver = new PrivateSetterContractResolver(),
			};

			_database = factory.GetRequiredService<ApplicationDbContext>();
			_database.RemoveRange(_database.Set<User>());
			_database.SaveChanges();
		}
		
		[Fact]
		public async Task Get_NoExistingUser_Returns404()
		{
			// Arrange
			var userId = Guid.NewGuid();

			// Act
			var response = await _httpClient.GetAsync($"Users/{userId}");

			// Assert
			response.IsSuccessStatusCode.Should().BeFalse();
			response.StatusCode.Should()
				.Be(HttpStatusCode.NotFound);
		}

		[Fact]
		public async Task GetOData_NoQueryStringNoUsers_ReturnsEmptyResponse()
		{
			// Arrange
			
			// Act
			var response = await _httpClient.GetAsync($"Users");

			// Assert
			response.IsSuccessStatusCode.Should().BeTrue();
			response.StatusCode.Should()
				.Be(HttpStatusCode.OK);
			var json = await response.Content.ReadAsStringAsync(); // TODO: get Stream instead of loading String
			var data = JsonConvert.DeserializeObject<PagedResponse<GetUsersResponse>>(json, _jsonSettings);
			data.Should().NotBeNull();
			data!.Items.Length.Should().Be(0);
		}
		
		[Fact]
		public async Task GetOData_Top1NoUsers_ReturnsEmptyResponse()
		{
			// Arrange
			
			// Act
			var response = await _httpClient.GetAsync($"Users?$top=1");

			// Assert
			response.IsSuccessStatusCode.Should().BeTrue();
			response.StatusCode.Should()
				.Be(HttpStatusCode.OK);
			var json = await response.Content.ReadAsStringAsync(); // TODO: get Stream instead of loading String
			var data = JsonConvert.DeserializeObject<PagedResponse<GetUsersResponse>>(json, _jsonSettings);
			data.Should().NotBeNull();
			data!.Items.Length.Should().Be(0);
		}
		
		[Fact(Skip = "OData Select should not be supported - use Mapping")]
		public async Task GetOData_SelectNameNoUsers_ReturnsEmptyResponse()
		{
			// Arrange
			_database.Add(Wally.CleanArchitecture.Domain.Users.User.Create("testUser1"));
			_database.Add(Wally.CleanArchitecture.Domain.Users.User.Create("testUser3"));
			_database.Add(Wally.CleanArchitecture.Domain.Users.User.Create("testUser2"));
			await _database.SaveChangesAsync();
			
			// Act
			var response = await _httpClient.GetAsync($"Users?$select=name");

			// Assert
			response.IsSuccessStatusCode.Should().BeTrue();
			response.StatusCode.Should()
				.Be(HttpStatusCode.OK);
			var json = await response.Content.ReadAsStringAsync(); // TODO: get Stream instead of loading String
			var data = JsonConvert.DeserializeObject<PagedResponse<GetUsersResponse>>(json, _jsonSettings);
			data.Should().NotBeNull();
			data!.Items.Length.Should().Be(0);
		}
		
		[Fact]
		public async Task GetOData_3Users_Returns3Users()
		{
			// Arrange
			_database.Add(Wally.CleanArchitecture.Domain.Users.User.Create("testUser1"));
			_database.Add(Wally.CleanArchitecture.Domain.Users.User.Create("testUser3"));
			_database.Add(Wally.CleanArchitecture.Domain.Users.User.Create("testUser2"));
			await _database.SaveChangesAsync();
			
			// Act
			var response = await _httpClient.GetAsync($"Users"); // 3
			
			// Assert
			response.IsSuccessStatusCode.Should().BeTrue();
			response.StatusCode.Should()
				.Be(HttpStatusCode.OK);
			var json = await response.Content.ReadAsStringAsync(); // TODO: get Stream instead of loading String
			var data = JsonConvert.DeserializeObject<PagedResponse<GetUsersResponse>>(json, _jsonSettings);
			data.Should().NotBeNull();
			data!.Items.Length.Should().Be(3);
		}
		
		[Fact]
		public async Task GetOData_3UsersOrdered_Returns3Users()
		{
			// Arrange
			_database.Add(Wally.CleanArchitecture.Domain.Users.User.Create("testUser1"));
			_database.Add(Wally.CleanArchitecture.Domain.Users.User.Create("testUser3"));
			_database.Add(Wally.CleanArchitecture.Domain.Users.User.Create("testUser2"));
			await _database.SaveChangesAsync();
			
			// Act
			var response = await _httpClient.GetAsync($"Users?$orderby=Name"); // 3
			
			// Assert
			response.IsSuccessStatusCode.Should().BeTrue();
			response.StatusCode.Should()
				.Be(HttpStatusCode.OK);
			var json = await response.Content.ReadAsStringAsync(); // TODO: get Stream instead of loading String
			var data = JsonConvert.DeserializeObject<PagedResponse<GetUsersResponse>>(json, _jsonSettings);
			data.Should().NotBeNull();
			data!.Items.Length.Should().Be(3);
			data.Items[0].Name.Should().Be("testUser1");
			data.Items[1].Name.Should().Be("testUser2");
			data.Items[2].Name.Should().Be("testUser3");
		}
		
		[Fact]
		public async Task GetOData_3UsersOrderedDesc_Returns3Users()
		{
			// Arrange
			_database.Add(Wally.CleanArchitecture.Domain.Users.User.Create("testUser1"));
			_database.Add(Wally.CleanArchitecture.Domain.Users.User.Create("testUser3"));
			_database.Add(Wally.CleanArchitecture.Domain.Users.User.Create("testUser2"));
			await _database.SaveChangesAsync();
			
			// Act
			var response = await _httpClient.GetAsync($"Users?$orderby=Name desc"); // 3
			
			// Assert
			response.IsSuccessStatusCode.Should().BeTrue();
			response.StatusCode.Should()
				.Be(HttpStatusCode.OK);
			var json = await response.Content.ReadAsStringAsync(); // TODO: get Stream instead of loading String
			var data = JsonConvert.DeserializeObject<PagedResponse<GetUsersResponse>>(json, _jsonSettings);
			data.Should().NotBeNull();
			data!.Items.Length.Should().Be(3);
			data.Items[0].Name.Should().Be("testUser3");
			data.Items[1].Name.Should().Be("testUser2");
			data.Items[2].Name.Should().Be("testUser1");
		}
		
		[Fact]
		public async Task GetOData_OrderSkip_ReturnsEmptyResponse()
		{
			// Arrange
			_database.Add(Wally.CleanArchitecture.Domain.Users.User.Create("testUser1"));
			_database.Add(Wally.CleanArchitecture.Domain.Users.User.Create("testUser3"));
			_database.Add(Wally.CleanArchitecture.Domain.Users.User.Create("testUser2"));
			await _database.SaveChangesAsync();
			
			// Act
			var response = await _httpClient.GetAsync($"Users?$orderby=Name&$skip=1"); // 1
			// var response = await _httpClient.GetAsync($"Users?$orderby=Name&$top=1");
			// var response = await _httpClient.GetAsync($"Users?$orderby=Name&$skip=1&$top=1");
			
			// Assert
			response.IsSuccessStatusCode.Should().BeTrue();
			response.StatusCode.Should()
				.Be(HttpStatusCode.OK);
			var json = await response.Content.ReadAsStringAsync(); // TODO: get Stream instead of loading String
			var data = JsonConvert.DeserializeObject<PagedResponse<GetUsersResponse>>(json, _jsonSettings);
			data.Should().NotBeNull();
			data!.Items.Length.Should().Be(2);
			data.Items[0].Name.Should().Be("testUser2");
			data.Items[1].Name.Should().Be("testUser3");
		}
		
		[Fact]
		public async Task GetOData_OrderTop2_ReturnsEmptyResponse()
		{
			// Arrange
			_database.Add(Wally.CleanArchitecture.Domain.Users.User.Create("testUser1"));
			_database.Add(Wally.CleanArchitecture.Domain.Users.User.Create("testUser3"));
			_database.Add(Wally.CleanArchitecture.Domain.Users.User.Create("testUser2"));
			await _database.SaveChangesAsync();
			
			// Act
			var response = await _httpClient.GetAsync($"Users?$orderby=Name&$top=2"); // 1
			
			// Assert
			response.IsSuccessStatusCode.Should().BeTrue();
			response.StatusCode.Should()
				.Be(HttpStatusCode.OK);
			var json = await response.Content.ReadAsStringAsync(); // TODO: get Stream instead of loading String
			var data = JsonConvert.DeserializeObject<PagedResponse<GetUsersResponse>>(json, _jsonSettings);
			data.Should().NotBeNull();
			data!.Items.Length.Should().Be(2);
			data.Items[0].Name.Should().Be("testUser1");
			data.Items[1].Name.Should().Be("testUser2");
		}
		
		[Fact]
		public async Task GetOData_OrderSkip1Top2_ReturnsEmptyResponse()
		{
			// Arrange
			_database.Add(Wally.CleanArchitecture.Domain.Users.User.Create("testUser1"));
			_database.Add(Wally.CleanArchitecture.Domain.Users.User.Create("testUser3"));
			_database.Add(Wally.CleanArchitecture.Domain.Users.User.Create("testUser2"));
			await _database.SaveChangesAsync();
			
			// Act
			var response = await _httpClient.GetAsync($"Users?$orderby=Name&$skip=1&$top=2"); // 1
			
			// Assert
			response.IsSuccessStatusCode.Should().BeTrue();
			response.StatusCode.Should()
				.Be(HttpStatusCode.OK);
			var json = await response.Content.ReadAsStringAsync(); // TODO: get Stream instead of loading String
			var data = JsonConvert.DeserializeObject<PagedResponse<GetUsersResponse>>(json, _jsonSettings);
			data.Should().NotBeNull();
			data!.Items.Length.Should().Be(2);
			data.Items[0].Name.Should().Be("testUser2");
			data.Items[1].Name.Should().Be("testUser3");
		}
	}
}
