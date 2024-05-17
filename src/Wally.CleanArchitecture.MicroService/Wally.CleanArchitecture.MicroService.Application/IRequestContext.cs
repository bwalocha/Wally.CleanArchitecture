using Wally.CleanArchitecture.MicroService.Domain;
using Wally.CleanArchitecture.MicroService.Domain.Users;

namespace Wally.CleanArchitecture.MicroService.Application;

public interface IRequestContext
{
	public CorrelationId CorrelationId { get; }
	
	public UserId UserId { get; }
}
