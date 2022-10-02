using System;

namespace Wally.CleanArchitecture.MicroService.Domain.Abstractions;

public interface IUserProvider
{
	Guid GetCurrentUserId();
}
