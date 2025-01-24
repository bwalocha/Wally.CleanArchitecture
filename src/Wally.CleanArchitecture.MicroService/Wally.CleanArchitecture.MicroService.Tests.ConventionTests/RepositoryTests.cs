﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Shouldly;
using Wally.CleanArchitecture.MicroService.Application.Abstractions;
using Wally.CleanArchitecture.MicroService.Infrastructure.Persistence;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Extensions;
using Xunit;

namespace Wally.CleanArchitecture.MicroService.Tests.ConventionTests;

public class RepositoryTests
{
	[Fact]
	public void Repository_ReturnedCollection_ShouldBeMaterialized()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.ImplementsGenericInterface(typeof(IReadOnlyRepository<,>)));
		var notAllowedTypes = new List<Type>
		{
			typeof(IEnumerable),
			typeof(IEnumerable<>),
			typeof(IQueryable),
			typeof(IQueryable<>),
		};

		types.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in types)
			{
				type.GetMethods()
					.Where(a => a.IsPublic || a.IsAssembly)
					.Select(a => a.ReturnType)
					.Where(
						a => notAllowedTypes.Contains(a) || notAllowedTypes.Exists(
							n =>
								Array.Exists(a.GenericTypeArguments, g => g.GetTypeDefinitionIfGeneric() == n)))
					.ShouldBeEmpty($"Repository '{type}' should return only materialized responses");
			}
		});
	}

	[Fact]
	public void Repository_Interfaces_ShouldBeInApplicationLayer()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.IsInterface && a.ImplementsGenericInterface(typeof(IReadOnlyRepository<,>)));

		types.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in types)
			{
				if (type == typeof(IReadOnlyRepository<,>))
				{
					continue;
				}

				if (type == typeof(IRepository<,>))
				{
					continue;
				}

				Configuration.Assemblies.Application.GetAllTypes().SingleOrDefault(a => a == type)
					.ShouldNotBeNull($"Repository '{type}' should be located in Application layer");
			}
		});
	}

	[Fact]
	public void Repository_Interfaces_ShouldHaveImplementationInPersistent()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies().ToArray();
		var types = assemblies.GetAllTypes()
			.Where(a => a.IsInterface && a.ImplementsGenericInterface(typeof(IReadOnlyRepository<,>)))
			.ToList();
		var repositories = typeof(IInfrastructurePersistenceAssemblyMarker).Assembly.GetTypes()
			.Where(a => a.ImplementsGenericInterface(typeof(IReadOnlyRepository<,>)))
			.ToList();

		types.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in types)
			{
				if (type == typeof(IReadOnlyRepository<,>))
				{
					continue;
				}

				if (type == typeof(IRepository<,>))
				{
					continue;
				}

				repositories.SingleOrDefault(a => a.ImplementsInterface(type))
					.ShouldNotBeNull($"Repository for '{type}' interface should be implemented, and '{0}' is not");
			}
		});
	}
}
