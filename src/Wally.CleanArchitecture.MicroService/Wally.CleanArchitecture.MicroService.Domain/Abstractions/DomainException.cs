using System;

namespace Wally.CleanArchitecture.MicroService.Domain.Abstractions;

public class DomainException : Exception
{
	public DomainException()
	{
	}
	
	public DomainException(string? message)
		: base(message)
	{
	}
	
	public DomainException(string? message, Exception? innerException)
		: base(message, innerException)
	{
	}
}
