using AutoMapper;
using Wally.CleanArchitecture.MicroService.Domain.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Application.MapperProfiles;

public class ValueObjectProfile : Profile
{
	public ValueObjectProfile()
	{
		CreateMap<ValueObject<string>, string>()
			.ConvertUsing(a => a.Value);
		
		CreateMap<ValueObject<int>, int>()
			.ConvertUsing(a => a.Value);
	}
}
