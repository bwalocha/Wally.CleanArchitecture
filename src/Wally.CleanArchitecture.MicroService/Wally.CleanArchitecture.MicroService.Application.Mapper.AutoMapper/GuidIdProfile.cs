using System;
using Wally.CleanArchitecture.MicroService.Domain.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Application.Mapper.AutoMapper;

public class GuidIdProfile : Profile
{
	public GuidIdProfile()
	{
		CreateMap<IStronglyTypedId<Guid>, Guid>()
			.ConvertUsing(a => a.Value);

		CreateMap<IStronglyTypedId<Guid>?, Guid?>()
			.ConvertUsing(a => a == null ? null : a.Value);
	}
}
