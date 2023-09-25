using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Types;

using Wally.CleanArchitecture.MicroService.Domain.Abstractions;
using Wally.CleanArchitecture.MicroService.Infrastructure.Persistence;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Helpers;

using Xunit;

namespace Wally.CleanArchitecture.MicroService.Tests.ConventionTests;

public class RepositoryTests
{
	[Fact]
	public void Repository_ReturnedCollection_ShouldBeMaterialized()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();

		using (new AssertionScope(new AssertionStrategy()))
		{
			foreach (var assembly in assemblies)
			{
				var types = AllTypes.From(assembly)
					.ThatSatisfy(a => a.ImplementsGenericInterface(typeof(IReadOnlyRepository<>)));

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

	[Fact]
	public void Repository_Interfaces_ShouldBeInApplicationLayer()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();

		using (new AssertionScope(new AssertionStrategy()))
		{
			foreach (var assembly in assemblies)
			{
				var types = AllTypes.From(assembly)
					.ThatSatisfy(a => a.IsInterface && a.ImplementsGenericInterface(typeof(IReadOnlyRepository<>)));

				foreach (var type in types)
				{
					if (type == typeof(IReadOnlyRepository<>))
					{
						continue;
					}

					if (type == typeof(IRepository<>))
					{
						continue;
					}

					Configuration.Assemblies.Application.Should()
						.Contain(
							type.Assembly,
							$"type '{type}' should be located in Application Layer, not in '{0}'",
							type.Assembly);
				}
			}
		}
	}

	[Fact]
	public void Repository_Interfaces_ShouldHaveImplementationInPersistent()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var repositories = AllTypes.From(typeof(IInfrastructurePersistenceAssemblyMarker).Assembly)
			.ThatSatisfy(a => a.ImplementsGenericInterface(typeof(IReadOnlyRepository<>)))
			.ToList();

		using (new AssertionScope(new AssertionStrategy()))
		{
			foreach (var assembly in assemblies)
			{
				var types = AllTypes.From(assembly)
					.ThatSatisfy(a => a.IsInterface && a.ImplementsGenericInterface(typeof(IReadOnlyRepository<>)))
					.ToList();

				foreach (var type in types)
				{
					if (type == typeof(IReadOnlyRepository<>))
					{
						continue;
					}

					if (type == typeof(IRepository<>))
					{
						continue;
					}

					repositories.ThatSatisfy(a => a.ImplementsInterface(type))
						.ToList()
						.Should()
						.NotBeEmpty("all Repositiory Interfaces should be implemented, and '{0}' is not", type);
				}
			}
		}
	}
}
