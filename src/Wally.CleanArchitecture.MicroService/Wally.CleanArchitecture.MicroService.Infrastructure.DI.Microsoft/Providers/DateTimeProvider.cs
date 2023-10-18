using System;

using Wally.CleanArchitecture.MicroService.Domain.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Providers;

public class DateTimeProvider : IDateTimeProvider
{
	private readonly Func<DateTimeOffset> _function;

	public DateTimeProvider()
	{
		_function = () => DateTimeOffset.UtcNow;
	}

	public DateTimeProvider(Func<DateTimeOffset> function)
	{
		_function = function;
	}

	public DateTimeOffset GetDateTime()
	{
		return _function();
	}
}
