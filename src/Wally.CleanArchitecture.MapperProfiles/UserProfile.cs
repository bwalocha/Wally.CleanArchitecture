using AutoMapper;

using Wally.CleanArchitecture.Contracts.Requests.Users;
using Wally.CleanArchitecture.Contracts.Responses.Users;
using Wally.CleanArchitecture.Domain.Users;

namespace Wally.CleanArchitecture.MapperProfiles;

public class UserProfile : Profile
{
	public UserProfile()
	{
		CreateMap<User, GetUsersRequest>();
		CreateMap<User, GetUsersResponse>();

		CreateMap<User, GetUserResponse>();

		// OData
		CreateMap<GetUsersRequest, GetUsersResponse>();
	}
}
