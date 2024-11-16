using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using VerifyXunit;
using Wally.CleanArchitecture.MicroService.Application.Contracts.Users.Requests;
using Wally.CleanArchitecture.MicroService.Domain.Users;
using Wally.CleanArchitecture.MicroService.Tests.IntegrationTests.Extensions;
using Xunit;

namespace Wally.CleanArchitecture.MicroService.Tests.IntegrationTests;

public partial class UsersControllerTests
{
	[Fact]
	public async Task Put_ForExistingResource_UpdatesResourceData()
	{
		// Arrange
		var resource = UserCreate(3);
		await _factory.SeedAsync(resource);
		var request = new UpdateUserRequest("newTestResource1");

		// Act
		var response = await _httpClient.PutAsync($"Users/{resource.Id.Value}", request, CancellationToken.None);

		// Assert
		await Verifier.Verify(response);

		(await _factory.GetRequiredService<DbContext>()
				.Set<User>()
				.SingleAsync(a => a.Id == resource.Id))
			.Name.Should()
			.Be("newTestResource1");
	}
}
