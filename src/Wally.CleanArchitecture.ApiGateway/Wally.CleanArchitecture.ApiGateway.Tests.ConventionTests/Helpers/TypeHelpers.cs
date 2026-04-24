using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Wally.CleanArchitecture.ApiGateway.Infrastructure.DI.Microsoft;
using Wally.CleanArchitecture.ApiGateway.WebApi;

namespace Wally.CleanArchitecture.ApiGateway.Tests.ConventionTests.Helpers;

public static class TypeHelpers
{
	private static readonly List<string> Prefixes =
	[
		"Wally.CleanArchitecture.ApiGateway",
	];

	public static IEnumerable<Assembly> GetAllInternalAssemblies()
	{
		var assemblies = typeof(IInfrastructureDIMicrosoftAssemblyMarker).Assembly.GetReferencedAssemblies()
			.Concat(
				typeof(Startup).Assembly.GetReferencedAssemblies())
			.Where(a => Prefixes.Exists(b => a.FullName.StartsWith(b)));

		foreach (var assembly in assemblies)
		{
			yield return Assembly.Load(assembly);
		}

		yield return typeof(Startup).Assembly;
	}
}
