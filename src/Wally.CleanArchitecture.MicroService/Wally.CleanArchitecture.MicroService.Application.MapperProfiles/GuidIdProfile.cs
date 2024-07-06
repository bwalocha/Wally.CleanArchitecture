using System;
using AutoMapper;
using Wally.CleanArchitecture.MicroService.Domain.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Application.MapperProfiles;

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
