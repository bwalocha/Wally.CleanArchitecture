using FluentAssertions;
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
}
