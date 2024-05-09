using System;
using Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.Exceptions;

public class ResourceNotFoundException : Exception, INotFound
{
	public ResourceNotFoundException()
	{
	}

	public ResourceNotFoundException(string? message)
		: base(message)
	{
	}

	public ResourceNotFoundException(string message, Exception? innerException)
		: base(message, innerException)
	{
	}
}
