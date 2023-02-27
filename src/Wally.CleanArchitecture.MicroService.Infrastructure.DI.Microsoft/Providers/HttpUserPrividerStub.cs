using System;

using Wally.CleanArchitecture.MicroService.Domain.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Providers;

// TODO: use userId form JWT Token
public class HttpUserProvider : IUserProvider
{
	private readonly Guid _userId = Guid.Parse("AAAAAAAA-0000-0000-0000-ADD702D3016B");

	public Guid GetCurrentUserId()
	{
		return _userId;
	}
}
