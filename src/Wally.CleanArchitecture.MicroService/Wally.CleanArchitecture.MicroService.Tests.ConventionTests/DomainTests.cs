﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Common;
using FluentAssertions.Execution;
using Wally.CleanArchitecture.MicroService.Domain.Abstractions;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Extensions;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Helpers;
using Xunit;

namespace Wally.CleanArchitecture.MicroService.Tests.ConventionTests;

public class DomainTests
{
	[Fact]
	public void Domain_Constructor_ShouldBePrivate()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.InheritsGenericClass(typeof(Entity<,>)))
			.Types();
		
		using (new AssertionScope(new AssertionStrategy()))
		{
			types.Should()
				.HaveOnlyPrivateParameterlessConstructor();
		}
	}
	
	[Fact]
	public void Domain_AggregateRootAndEntity_ShouldNotExposeSetter()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.InheritsGenericClass(typeof(Entity<,>)))
			.Types();
		
		using (new AssertionScope())
		{
			foreach (var type in types)
			{
				foreach (var property in type.Properties())
				{
					if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType) &&
						property.PropertyType != typeof(string))
					{
						property.Should()
							.NotBeWritable("Entity '{0}' should not expose setter '{1}'", type, property);
					}
					else if (property.CanWrite)
					{
						property.Should()
							.BeWritable(
								CSharpAccessModifier.Private,
								"Entity '{0}' should not expose writable setter '{1}'",
								type,
								property);
					}
				}
			}
		}
	}
	
	[Fact]
	public void Domain_AggregateRootAndEntity_ShouldNotExposeWritableCollection()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.InheritsGenericClass(typeof(Entity<,>)))
			.Types();
		
		using (new AssertionScope())
		{
			foreach (var type in types)
			{
				foreach (var property in type.Properties())
				{
					if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType) &&
						property.PropertyType != typeof(string))
					{
						property.PropertyType.GetGenericTypeDefinition()
							.Should()
							.Be(
								typeof(IReadOnlyCollection<>),
								"Entity '{0}' should not expose writable collection '{1}'",
								type,
								property);
					}
				}
			}
		}
	}
	
	[Fact]
	public void Domain_ValueObject_ShouldNotExposeSetter()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.InheritsGenericClass(typeof(ValueObject<>)))
			.Types();
		
		using (new AssertionScope())
		{
			foreach (var type in types)
			{
				foreach (var property in type.Properties())
				{
					property.Should()
						.NotBeWritable("ValueObject '{0}' should not expose setter '{1}'", type, property);
				}
			}
		}
	}
}
