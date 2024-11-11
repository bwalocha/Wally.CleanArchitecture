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

		// CreateMap<UserId, Guid?>()
		// 	.ConvertUsing(a => a.Value);
		
		// CreateMap<UserId?, Guid>()
		// 	.ConvertUsing(a => a == null ? Guid.NewGuid() : a.Value);

		/*CreateMap<UserId?, Guid?>()
			.ConvertUsing(a => a == null ? null : a.Value);*/
		
		CreateMap<Guid, UserId>()
			.ConvertUsing(a => new UserId(a));

		// CreateMap<Guid, UserId?>()
		// 	.ConvertUsing(a => new UserId(a));
		
		/*CreateMap<Guid?, UserId>()
			.ConvertUsing(a => a == null ? new UserId() : new UserId(a.Value));
		
		CreateMap<Guid?, UserId?>()
			.ConvertUsing(a => a == null ? null : new UserId(a.Value));*/
	}
}
