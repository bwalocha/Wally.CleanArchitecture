using Wally.CleanArchitecture.MicroService.Domain.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Application.MapperProfiles;

public class IntegerIdProfile : Profile
{
	public IntegerIdProfile()
	{
		CreateMap<IStronglyTypedId<int>, int>()
			.ConvertUsing(a => a.Value);

		CreateMap<IStronglyTypedId<int>?, int?>()
			.ConvertUsing(a => a == null ? null : a.Value);
	}
}
