using System;

namespace Wally.CleanArchitecture.MicroService.Domain.Abstractions;

public interface IDateTimeProvider
{
	public DateTime GetDateTime();
}
