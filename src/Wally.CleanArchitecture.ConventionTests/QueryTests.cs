using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Types;
using Wally.CleanArchitecture.Application.Users.Queries;
using Wally.Lib.DDD.Abstractions.Queries;
using Xunit;

namespace Wally.CleanArchitecture.ConventionTests
{
	public class QueryTests
	{
		[Fact]
		public void Application_Query_ShouldNotExposeSetter()
		{
			var applicationTypes = AllTypes.From(typeof(GetUserQuery).Assembly);

			applicationTypes.Where(a => typeof(IQuery<>).IsAssignableFrom(a))
				.Types()
				.Properties()
				.Should()
				.NotBeWritable("query should be immutable");
		}

		[Fact]
		public void Application_Query_ShouldBeExcludedFromCodeCoverage()
		{
			var applicationTypes = AllTypes.From(typeof(GetUserQuery).Assembly);

			applicationTypes.Where(a => typeof(IQuery<>).IsAssignableFrom(a))
				.Types()
				.Should()
				.BeDecoratedWith<ExcludeFromCodeCoverageAttribute>();
		}

		[Fact]
		public void Application_Query_ShouldBeSealed()
		{
			var applicationTypes = AllTypes.From(typeof(GetUserQuery).Assembly);

			applicationTypes.Where(a => typeof(IQuery<>).IsAssignableFrom(a))
				.Types()
				.Should()
				.BeSealed();
		}
	}
}
