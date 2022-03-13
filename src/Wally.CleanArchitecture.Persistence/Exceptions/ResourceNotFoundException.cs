using System;

using Wally.CleanArchitecture.Persistence.Abstractions;

namespace Wally.CleanArchitecture.Persistence.Exceptions;

public class ResourceNotFoundException : Exception, INotFound
{
}
