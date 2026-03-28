using System.Diagnostics.CodeAnalysis;

namespace Wally.CleanArchitecture.MicroService.Domain.Exceptions;

[ExcludeFromCodeCoverage]
public class PermissionDeniedException : DomainException
{
	public PermissionDeniedException(string message)
		: base(message)
	{
	}
}
