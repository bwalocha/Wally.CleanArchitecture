using System.Diagnostics.CodeAnalysis;
using Wally.CleanArchitecture.MicroService.Domain.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Domain.Exceptions;

[ExcludeFromCodeCoverage]
public class PermissionDeniedException : DomainException
{
	public PermissionDeniedException(string message)
		: base(message)
	{
	}
}
