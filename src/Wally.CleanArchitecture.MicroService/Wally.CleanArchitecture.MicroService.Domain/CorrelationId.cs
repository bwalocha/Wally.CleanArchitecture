using System;
using Wally.CleanArchitecture.MicroService.Domain.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Domain;

public class CorrelationId : GuidId<CorrelationId>
{
	public CorrelationId(Guid value)
		: base(value)
	{
	}
}
