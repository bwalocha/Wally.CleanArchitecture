using System;
using Wally.CleanArchitecture.MicroService.Domain.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Domain;

public sealed class CorrelationId : GuidId<CorrelationId>
{
	public CorrelationId(Guid value)
		: base(value)
	{
	}
	
	public static explicit operator Guid(CorrelationId id)
	{
		return id.Value;
	}
}
