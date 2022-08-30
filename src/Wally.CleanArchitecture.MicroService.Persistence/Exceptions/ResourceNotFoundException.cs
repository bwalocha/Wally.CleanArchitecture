using System;

using Wally.CleanArchitecture.MicroService.Persistence.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Persistence.Exceptions;

public class ResourceNotFoundException : Exception, INotFound
{
	public ResourceNotFoundException(string message, Exception? innerException)
		: base(message, innerException)
	{
	}
}
