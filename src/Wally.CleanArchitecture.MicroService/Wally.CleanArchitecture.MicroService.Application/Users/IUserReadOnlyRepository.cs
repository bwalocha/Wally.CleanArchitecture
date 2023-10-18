using Wally.CleanArchitecture.MicroService.Domain.Abstractions;
using Wally.CleanArchitecture.MicroService.Domain.Users;

namespace Wally.CleanArchitecture.MicroService.Application.Users;

public interface IUserReadOnlyRepository : IReadOnlyRepository<User, UserId>
{
}
