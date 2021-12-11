using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Types;
using Wally.CleanArchitecture.Domain.Users;
using Wally.CleanArchitecture.ConventionTests.Helpers;
using Wally.Lib.DDD.Abstractions.DomainModels;
using Xunit;

namespace Wally.CleanArchitecture.ConventionTests
{
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
}
