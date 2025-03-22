using Wally.CleanArchitecture.MicroService.Domain.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Application.MapperProfiles;

public class LongIdProfile : Profile
{
	public LongIdProfile()
	{
		CreateMap<IStronglyTypedId<long>, long>()
			.ConvertUsing(a => a.Value);

		CreateMap<IStronglyTypedId<long>?, long?>()
			.ConvertUsing(a => a == null ? null : a.Value);
	}
}
