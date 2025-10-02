using Wally.CleanArchitecture.MicroService.Application.Users.Requests;
using Wally.CleanArchitecture.MicroService.Application.Users.Results;
using Wally.CleanArchitecture.MicroService.Domain.Users;

namespace Wally.CleanArchitecture.MicroService.Application.MapperProfiles.Users;

public class UserProfile : Profile
{
	public UserProfile()
	{
		CreateMap<User, GetUsersRequest>();
		CreateMap<User, GetUsersResult>();

		CreateMap<User, GetUserResult>();
	}
}
