using Wally.CleanArchitecture.MicroService.Domain;
using Wally.CleanArchitecture.MicroService.Domain.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Application.Abstractions;

public interface IRequestContext
{
	public CorrelationId CorrelationId { get; }

	public UserId UserId { get; }
}
