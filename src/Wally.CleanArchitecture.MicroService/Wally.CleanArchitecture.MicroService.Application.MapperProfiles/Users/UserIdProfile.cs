using System;
using AutoMapper;
using Wally.CleanArchitecture.MicroService.Domain.Users;

namespace Wally.CleanArchitecture.MicroService.Application.MapperProfiles.Users;

public class UserIdProfile : Profile
{
	public UserIdProfile()
	{
		CreateMap<UserId, Guid>()
			.ConvertUsing(a => a.Value);

		CreateMap<Guid, UserId>()
			.ConvertUsing(a => new UserId(a));
	}
}
