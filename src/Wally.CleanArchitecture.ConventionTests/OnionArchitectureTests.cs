using System.Collections;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Common;
using FluentAssertions.Execution;
using FluentAssertions.Types;
using Wally.CleanArchitecture.Application.Users.Queries;
using Wally.CleanArchitecture.Domain.Users;
using Wally.CleanArchitecture.MapperProfiles;
using Wally.CleanArchitecture.Persistence;
using Wally.Lib.DDD.Abstractions.DomainModels;
using Xunit;

namespace Wally.CleanArchitecture.ConventionTests;

public class OnionArchitectureTests
{
	[Fact]
	public void Domain_IsNotReferencedByApplication()
	{
		var domainTypes = new[] { typeof(User), };
		var applicationTypes = new[] { typeof(GetUserQueryHandler), };

		using (new AssertionScope())
		{
			domainTypes.Select(a => a.Assembly)
				.Should()
				.SatisfyRespectively(
					a =>
					{
						foreach (var type in applicationTypes)
						{
							a.Should()
								.NotReference(type.Assembly);
						}
					});
		}
	}

	[Fact]
	public void Domain_IsNotReferencedByInfrastructure()
	{
		var domainTypes = new[] { typeof(User), };
		var infrastructureTypes = new[] { typeof(UserProfile), };

		using (new AssertionScope())
		{
			domainTypes.Select(a => a.Assembly)
				.Should()
				.SatisfyRespectively(
					a =>
					{
						foreach (var type in infrastructureTypes)
						{
							a.Should()
								.NotReference(type.Assembly);
						}
					});
		}
	}

	[Fact]
	public void Domain_IsNotReferencedByPersistence()
	{
		var domainTypes = new[] { typeof(User), };
		var persistenceTypes = new[] { typeof(ApplicationDbContext), };

		using (new AssertionScope())
		{
			domainTypes.Select(a => a.Assembly)
				.Should()
				.SatisfyRespectively(
					a =>
					{
						foreach (var type in persistenceTypes)
						{
							a.Should()
								.NotReference(type.Assembly);
						}
					});
		}
	}

	[Fact]
	public void Domain_Entity_ShouldNotExposeSetter()
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
	public void Domain_ValueObject_ShouldNotExposeSetter()
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
