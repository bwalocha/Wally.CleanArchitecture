using System;

using Wally.CleanArchitecture.MicroService.Domain.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Providers;

public class DateTimeProvider : IDateTimeProvider
{
	private readonly Func<DateTime> _function;

	public DateTimeProvider()
	{
		_function = () => DateTime.UtcNow;
	}

	public DateTimeProvider(Func<DateTime> function)
	{
		_function = function;
	}

	public DateTime GetDateTime()
	{
		return _function();
	}
}
