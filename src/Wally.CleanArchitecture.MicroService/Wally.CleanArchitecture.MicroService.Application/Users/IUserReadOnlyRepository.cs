using Wally.CleanArchitecture.MicroService.Application.Abstractions;
using Wally.CleanArchitecture.MicroService.Domain.Users;

namespace Wally.CleanArchitecture.MicroService.Application.Users;

public interface IUserReadOnlyRepository : IReadOnlyRepository<User, UserId>
{
}
