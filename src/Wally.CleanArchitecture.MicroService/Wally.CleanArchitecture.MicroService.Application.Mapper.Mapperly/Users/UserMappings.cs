using Wally.CleanArchitecture.MicroService.Application.Users.Requests;
using Wally.CleanArchitecture.MicroService.Application.Users.Results;
using Wally.CleanArchitecture.MicroService.Domain.Users;

namespace Wally.CleanArchitecture.MicroService.Application.Mapper.Mapperly.Users;

[Mapper(
	EnumMappingStrategy = EnumMappingStrategy.ByName,
	RequiredMappingStrategy = RequiredMappingStrategy.None)]
public partial class UserMappings
{
	public partial GetUsersRequest ToGetUsersRequest(User model);
	public partial GetUsersResult ToGetUsersResult(User model);

	public partial GetUserResult ToGetUserResult(User model);
}
