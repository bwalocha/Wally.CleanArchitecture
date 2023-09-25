using System;

namespace Wally.CleanArchitecture.MicroService.Domain.Abstractions;

public abstract class DomainException : Exception
{
	public DomainException(string message)
		: base(message)
	{
	}
}
