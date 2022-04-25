using System.Collections;
using System.Collections.Generic;

using FluentAssertions;
using FluentAssertions.Common;
using FluentAssertions.Execution;
using FluentAssertions.Types;

using Wally.CleanArchitecture.MicroService.ConventionTests.Helpers;
using Wally.CleanArchitecture.MicroService.Domain.Users;
using Wally.Lib.DDD.Abstractions.DomainModels;

using Xunit;

namespace Wally.CleanArchitecture.MicroService.ConventionTests;

public class EntityTests
{
	[Fact]
	public void Entity_Constructor_ShouldBePrivate()
	{
		var applicationTypes = AllTypes.From(typeof(User).Assembly);

		using (new AssertionScope(new AssertionStrategy()))
		{
			applicationTypes.ThatImplement<Entity>()
				.Should()
				.HaveOnlyPrivateParameterlessConstructor();
		}
	}

	[Fact]
	public void Entity_AggregateRootAndEntity_ShouldNotExposeSetter()
	{
		var domainTypes = AllTypes.From(typeof(User).Assembly);

		using (new AssertionScope())
		{
			foreach (var type in domainTypes.ThatImplement<Entity>())
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
								"Entity '{0}' should not expose setter '{1}'",
								type,
								property);
					}
				}
			}
		}
	}

	[Fact]
	public void Entity_AggregateRootAndEntity_ShouldNotExposeWritableCollection()
	{
		var domainTypes = AllTypes.From(typeof(User).Assembly);

		using (new AssertionScope())
		{
			foreach (var type in domainTypes.ThatImplement<Entity>())
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
	public void Entity_ValueObject_ShouldNotExposeSetter()
	{
		var domainTypes = AllTypes.From(typeof(User).Assembly);

		using (new AssertionScope())
		{
			foreach (var type in domainTypes.ThatImplement<ValueObject>())
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
