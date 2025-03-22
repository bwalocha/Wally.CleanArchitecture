using Wally.CleanArchitecture.MicroService.Domain.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Application.MapperProfiles;

public class StringIdProfile : Profile
{
	public StringIdProfile()
	{
		CreateMap<IStronglyTypedId<string>, string>()
			.ConvertUsing(a => a.Value);

		/*CreateMap<IStronglyTypedId<string>?, string?>()
			.ConvertUsing(a => a == null ? null : a.Value);*/
	}
}
