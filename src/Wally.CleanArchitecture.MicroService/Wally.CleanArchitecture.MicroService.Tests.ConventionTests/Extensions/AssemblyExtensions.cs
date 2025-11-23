using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Extensions;

public static class AssemblyExtensions
{
	public static IEnumerable<Type> GetAllTypes(this IEnumerable<Assembly> assemblies)
	{
		return assemblies
			.SelectMany(a => a.GetTypes())
			.Where(a => !a.GetCustomAttributes<GeneratedCodeAttribute>()
				.Any());
	}
}
