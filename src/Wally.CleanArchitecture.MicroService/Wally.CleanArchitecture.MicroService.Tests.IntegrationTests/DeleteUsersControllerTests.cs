using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Time.Testing;
using VerifyXunit;
using Wally.CleanArchitecture.MicroService.Domain.Users;
using Wally.CleanArchitecture.MicroService.Tests.IntegrationTests.Extensions;
using Xunit;

namespace Wally.CleanArchitecture.MicroService.Tests.IntegrationTests;

public partial class UsersControllerTests
{
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
		timeProvider.SetUtcNow(new DateTime(2024, 12, 31, 16, 20, 00, DateTimeKind.Utc).ToDateTimeOffset());

		var resource1 = UserCreate(1);
		var resource2 = UserCreate(2);
		await _factory.SeedAsync(resource1, resource2);

		// Act
		var response = await _httpClient.DeleteAsync($"Users/{resource2.Id.Value}", CancellationToken.None);

		// Assert
		await Verifier.Verify(response);

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
		(await _factory.GetRequiredService<DbContext>()
				.Set<User>()
				.IgnoreQueryFilters()
				.SingleAsync(a => a.Id == resource2.Id))
			.DeletedById.Should()
			.Be(new UserId(Guid.Parse("aaaaaaaa-0000-0000-0000-add702d3016b")));
		(await _factory.GetRequiredService<DbContext>()
				.Set<User>()
				.IgnoreQueryFilters()
				.SingleAsync(a => a.Id == resource2.Id))
			.DeletedAt.Should()
			.Be(timeProvider.GetUtcNow());
	}
}
