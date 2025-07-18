using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Helpers;

public class Assemblies
{
	public Assembly[] Application { get; init; } = null!;

	public Assembly[] Domain { get; init; } = null!;

	public Assembly[] Infrastructure { get; init; } = null!;

	public Assembly[] Presentation { get; init; } = null!;

	public IEnumerable<Assembly> GetAllAssemblies()
	{
		return Domain
			.Concat(Application)
			.Concat(Infrastructure)
			.Concat(Presentation);
	}
}
