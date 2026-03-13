using Wally.CleanArchitecture.MicroService.Domain.Users;
using Wally.CleanArchitecture.MicroService.WebApi.Contracts.Users.Requests;

namespace Wally.CleanArchitecture.MicroService.WebApi.Mapper.AutoMapper.Users;

public class UserMappings : Profile
{
	public UserMappings()
	{
		CreateMap<User, GetUsersRequest>();
	}
}
