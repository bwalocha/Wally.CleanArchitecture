using Wally.CleanArchitecture.MicroService.Domain.Users;
using Wally.CleanArchitecture.MicroService.WebApi.Contracts.Users.Requests;
using Wally.CleanArchitecture.MicroService.WebApi.Contracts.Users.Responses;

namespace Wally.CleanArchitecture.MicroService.WebApi.Mapper.Mapperly.Users;

[Mapper(
	EnumMappingStrategy = EnumMappingStrategy.ByName,
	RequiredMappingStrategy = RequiredMappingStrategy.None)]
public partial class UserMappings
{
	public partial GetUsersRequest ToGetUsersRequest(User model);
	public partial GetUsersResponse ToGetUsersResponse(User model);

	public partial GetUserResponse ToGetUserResponse(User model);
}
