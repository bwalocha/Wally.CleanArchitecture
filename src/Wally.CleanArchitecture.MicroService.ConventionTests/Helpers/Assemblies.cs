using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Wally.CleanArchitecture.MicroService.ConventionTests.Helpers;

public class Assemblies
{
	public Assembly[] Application { get; init; }

	public Assembly[] Domain { get; init; }

	public Assembly[] Infrastructure { get; init; }

	public Assembly[] Presentation { get; init; }

	public IEnumerable<Assembly> GetAllAssemblies()
	{
		return Application.Concat(Domain)
			.Concat(Infrastructure)
			.Concat(Presentation);
	}
}
