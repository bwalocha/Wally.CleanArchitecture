using Wally.CleanArchitecture.MicroService.Domain.Users;
using Wally.CleanArchitecture.MicroService.WebApi.Contracts.Users.Requests;

namespace Wally.CleanArchitecture.MicroService.WebApi.Mapper.Mapster.Users;

public class UserMappings : IRegister
{
	public void Register(TypeAdapterConfig config)
	{
		config.NewConfig<User, GetUsersRequest>();
	}
}
