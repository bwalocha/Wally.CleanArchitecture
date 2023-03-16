using System;

using Wally.CleanArchitecture.MicroService.Domain.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Providers;

public class DateTimeProvider : IDateTimeProvider
{
	public DateTime GetDateTime()
	{
		return DateTime.UtcNow;
	}
}
