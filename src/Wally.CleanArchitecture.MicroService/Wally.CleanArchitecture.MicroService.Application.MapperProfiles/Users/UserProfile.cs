using Wally.CleanArchitecture.MicroService.Application.Users.Requests;
using Wally.CleanArchitecture.MicroService.Application.Users.Results;
using Wally.CleanArchitecture.MicroService.Domain.Users;

namespace Wally.CleanArchitecture.MicroService.Application.MapperProfiles.Users;

public class UserProfile : Profile
{
	// TODO: Use CreateProjection instead of CreateMap
	// https://docs.automapper.io/en/latest/11.0-Upgrade-Guide.html#createprojection
	public UserProfile()
	{
		CreateProjection<User, GetUsersRequest>();
		CreateProjection<User, GetUsersResult>();

		CreateProjection<User, GetUserResult>();
	}
}
