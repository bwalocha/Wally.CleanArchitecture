using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Wally.CleanArchitecture.MicroService.ConventionTests.Helpers;

public class Assemblies
{
	public Assembly[] Application { get; init; } = null!;

	public Assembly[] Domain { get; init; } = null!;

	public Assembly[] Infrastructure { get; init; } = null!;

	public Assembly[] Presentation { get; init; } = null!;

	public IEnumerable<Assembly> GetAllAssemblies()
	{
		return Application.Concat(Domain)
			.Concat(Infrastructure)
			.Concat(Presentation);
	}
}
