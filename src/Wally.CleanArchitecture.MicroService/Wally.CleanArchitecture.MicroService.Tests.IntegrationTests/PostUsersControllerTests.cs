using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
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
	public async Task Post_ForNewResource_CreatesNewResource()
	{
		// Arrange
		var request = new CreateUserRequest("newName3");

		// Act
		var response = await _httpClient.PostAsync("Users", request, CancellationToken.None);

		// Assert
		using var scope = new AssertionScope();
		await Verifier.Verify(response);

		(await _factory.GetRequiredService<DbContext>()
				.Set<User>()
				.SingleAsync())
			.Name.Should()
			.Be("newName3");
	}

	[Fact]
	public async Task Post_ForInvalidRequest_ReturnsBadRequest()
	{
		// Arrange
		var request = new CreateUserRequest(string.Empty);

		// Act
		var response = await _httpClient.PostAsync("Users", request, CancellationToken.None);

		// Assert
		using var scope = new AssertionScope();
		await Verifier.Verify(response);

		_factory.GetRequiredService<DbContext>()
			.Set<User>()
			.Should()
			.BeEmpty();
	}
}
