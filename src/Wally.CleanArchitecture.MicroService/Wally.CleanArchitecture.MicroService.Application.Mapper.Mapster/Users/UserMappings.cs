using Wally.CleanArchitecture.MicroService.Application.Users.Requests;
using Wally.CleanArchitecture.MicroService.Application.Users.Results;
using Wally.CleanArchitecture.MicroService.Domain.Users;

namespace Wally.CleanArchitecture.MicroService.Application.Mapper.Mapster.Users;

public class UserMappings : IRegister
{
	public void Register(TypeAdapterConfig config)
	{
		config.NewConfig<User, GetUsersRequest>();
		config.NewConfig<User, GetUsersResult>();

		config.NewConfig<User, GetUserResult>();
	}
}
