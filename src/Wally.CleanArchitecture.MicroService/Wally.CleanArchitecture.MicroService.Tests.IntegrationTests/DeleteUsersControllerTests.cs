using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Time.Testing;
using Shouldly;
using Wally.CleanArchitecture.MicroService.Domain.Users;
using Wally.CleanArchitecture.MicroService.Infrastructure.Persistence;
using Wally.CleanArchitecture.MicroService.Tests.IntegrationTests.Extensions;
// TODO: replace with Verify

namespace Wally.CleanArchitecture.MicroService.Tests.IntegrationTests;

public partial class UsersControllerTests
{
	[Fact]
	public async Task Delete_ForNonExistingResource_ReturnsResourceNotFound()
	{
		// Arrange
		var resource = UserCreate(3);

		// Act
		var response = await _httpClient.DeleteAsync($"Users/{resource.Id.Value}", CancellationToken.None);

		// Assert
		await Verifier.Verify(response);
	}

	[Fact]
	public async Task Delete_ByTheSameUser_IsForbidden()
	{
		// Arrange
		var resource1 = User
			.Create(new UserId(Guid.Parse("FFFFFFFF-0000-0000-0000-ADD702D3016B")), "testUser1")
			.SetCreatedById(new UserId());
		var resource2 = UserCreate(2);
		await _factory.SeedAsync(resource1, resource2);

		// Act
		var response = await _httpClient.DeleteAsync($"Users/{resource1.Id.Value}", CancellationToken.None);

		// Assert
		await Verifier.Verify(response);
	}

	[Fact]
	public async Task Delete_ForExistingResource_SoftDeletesResourceData()
	{
		// Arrange
		var timeProvider = (FakeTimeProvider)_factory.GetRequiredService<TimeProvider>();
		timeProvider.SetUtcNow(new DateTime(2024, 12, 31, 16, 20, 00, DateTimeKind.Utc));

		var resource1 = UserCreate(1);
		var resource2 = UserCreate(2);
		await _factory.SeedAsync(resource1, resource2);

		// Act
		var response = await _httpClient.DeleteAsync($"Users/{resource2.Id.Value}", CancellationToken.None);

		// Assert
		// TODO: use ShouldSatisfyAllConditions
		await Verifier.Verify(response);
		(await _factory.GetRequiredService<ApplicationDbContext>()
				.Set<User>()
				.SingleAsync(a => a.Id == resource1.Id))
			.IsDeleted.ShouldBe(false);
		(await _factory.GetRequiredService<ApplicationDbContext>()
				.Set<User>()
				.FirstOrDefaultAsync(a => a.Id == resource2.Id))
			.ShouldBeNull();
		(await _factory.GetRequiredService<ApplicationDbContext>()
				.Set<User>()
				.IgnoreQueryFilters()
				.SingleAsync(a => a.Id == resource1.Id))
			.IsDeleted.ShouldBe(false);
		(await _factory.GetRequiredService<ApplicationDbContext>()
				.Set<User>()
				.IgnoreQueryFilters()
				.SingleAsync(a => a.Id == resource2.Id))
			.IsDeleted.ShouldBe(true);
		(await _factory.GetRequiredService<ApplicationDbContext>()
				.Set<User>()
				.IgnoreQueryFilters()
				.SingleAsync(a => a.Id == resource2.Id))
			.DeletedById.ShouldBe(new UserId(Guid.Parse("ffffffff-0000-0000-0000-add702d3016b")));
		(await _factory.GetRequiredService<ApplicationDbContext>()
				.Set<User>()
				.IgnoreQueryFilters()
				.SingleAsync(a => a.Id == resource2.Id))
			.DeletedAt.ShouldBe(timeProvider.GetUtcNow());
	}
}
