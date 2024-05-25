using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Wally.CleanArchitecture.MicroService.Domain.Users;
using Xunit;

namespace Wally.CleanArchitecture.MicroService.Tests.IntegrationTests;

public partial class UsersControllerTests
{
	[Fact]
	public async Task Delete_ForExistingResource_SoftDeletesResourceData()
	{
		// Arrange
		var resource1 = UserCreate(1);
		var resource2 = UserCreate(2);
		await _factory.SeedAsync(resource1, resource2);
		
		// Act
		var response = await _httpClient.DeleteAsync($"Users/{resource2.Id.Value}", CancellationToken.None);
		
		// Assert
		response.IsSuccessStatusCode.Should()
			.BeTrue();
		response.StatusCode.Should()
			.Be(HttpStatusCode.Accepted);
		(await _factory.GetRequiredService<DbContext>()
				.Set<User>()
				.SingleAsync(a => a.Id == resource1.Id))
			.IsDeleted.Should()
			.Be(false);
		(await _factory.GetRequiredService<DbContext>()
				.Set<User>()
				.FirstOrDefaultAsync(a => a.Id == resource2.Id))
			.Should()
			.BeNull();
		(await _factory.GetRequiredService<DbContext>()
				.Set<User>()
				.IgnoreQueryFilters()
				.SingleAsync(a => a.Id == resource1.Id))
			.IsDeleted.Should()
			.Be(false);
		(await _factory.GetRequiredService<DbContext>()
				.Set<User>()
				.IgnoreQueryFilters()
				.SingleAsync(a => a.Id == resource2.Id))
			.IsDeleted.Should()
			.Be(true);
	}
}
