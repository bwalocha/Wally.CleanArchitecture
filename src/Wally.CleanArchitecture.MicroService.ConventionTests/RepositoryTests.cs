using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Types;

using Wally.CleanArchitecture.MicroService.ConventionTests.Helpers;
using Wally.CleanArchitecture.MicroService.Domain.Abstractions;

using Xunit;

namespace Wally.CleanArchitecture.MicroService.ConventionTests;

public class RepositoryTests
{
	[Fact]
	public void Repository_ReturnedCollection_ShouldBeMaterialized()
	{
		using (new AssertionScope(new AssertionStrategy()))
		{
			foreach (var assembly in TypeHelpers.GetAllInternalAssemblies())
			{
				var types = AllTypes.From(assembly)
					.ThatSatisfy(
						a => a.GetInterfaces()
							.Any(i => i.GetTypeDefinitionIfGeneric() == typeof(IRepository<>)));

				var notAllowedTypes = new List<Type>
				{
					typeof(IEnumerable), typeof(IEnumerable<>), typeof(IQueryable), typeof(IQueryable<>),
				};

				foreach (var type in types)
				{
					type.Methods()
						.ThatArePublicOrInternal.ReturnTypes()
						.ThatSatisfy(
							a => notAllowedTypes.Contains(a) || notAllowedTypes.Any(
								n => a.GenericTypeArguments.Any(g => g.GetTypeDefinitionIfGeneric() == n)))
						.ToArray()
						.Should()
						.BeEmpty("do not return not materialized collections from Repository '{0}'", type);
				}
			}
		}
	}
}
