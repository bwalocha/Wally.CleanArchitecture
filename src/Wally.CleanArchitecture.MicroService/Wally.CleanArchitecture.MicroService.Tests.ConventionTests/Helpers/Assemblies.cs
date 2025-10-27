using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Helpers;

public class Assemblies
{
	public required Assembly[] Application { get; init; } = null!;

	public required Assembly[] Domain { get; init; } = null!;

	public required Assembly[] Infrastructure { get; init; } = null!;

	public required Assembly[] Presentation { get; init; } = null!;

	public IEnumerable<Assembly> GetAllAssemblies()
	{
		return Domain
			.Concat(Application)
			.Concat(Infrastructure)
			.Concat(Presentation);
	}
}
