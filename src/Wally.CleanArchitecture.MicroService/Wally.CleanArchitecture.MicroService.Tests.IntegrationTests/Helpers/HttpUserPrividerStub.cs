using System;
using Wally.CleanArchitecture.MicroService.Domain.Abstractions;
using Wally.CleanArchitecture.MicroService.Domain.Users;

namespace Wally.CleanArchitecture.MicroService.Tests.IntegrationTests.Helpers;

public class HttpUserProviderStub : IUserProvider
{
	private readonly UserId _userId = new(Guid.Parse("FFFFFFFF-0000-0000-0000-ADD702D3016B"));
	
	public UserId GetCurrentUserId()
	{
		return _userId;
	}
}
