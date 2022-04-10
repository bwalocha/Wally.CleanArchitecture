using Wally.CleanArchitecture.Domain.Abstractions;
using Wally.CleanArchitecture.Domain.Users;

namespace Wally.CleanArchitecture.Application.Users;

public interface IUserReadOnlyRepository : IReadOnlyRepository<User>
{
}
