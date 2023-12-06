using System;
using Wally.CleanArchitecture.MicroService.Domain.Abstractions;
using Wally.CleanArchitecture.MicroService.Domain.Users;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.BackgroundServices.Providers;

public class ServiceUserProvider : IUserProvider
{
	private readonly UserId _userId = new(Guid.Parse("AAAAAAAA-0000-0000-0000-ADD702D3016B"));

	public UserId GetCurrentUserId()
	{
		return _userId;
	}
}
