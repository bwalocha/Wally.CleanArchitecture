using System;
using System.Diagnostics.CodeAnalysis;

namespace Wally.CleanArchitecture.MicroService.Domain.Abstractions;

[ExcludeFromCodeCoverage]
public abstract class DomainException : Exception
{
	protected DomainException(string? message)
		: base(message)
	{
	}

	protected DomainException(string? message, Exception? innerException)
		: base(message, innerException)
	{
	}
}
