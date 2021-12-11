using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using Wally.CleanArchitecture.ConventionTests.Helpers;
using Wally.Lib.DDD.Abstractions.DomainEvents;
using Xunit;

namespace Wally.CleanArchitecture.ConventionTests
{
	public class DomainEventTests
	{
		[Fact]
		public void Domain_ClassesWhichInheritsDomainEvent_ShouldBeInDomainProject()
		{
			using (new AssertionScope(new AssertionStrategy()))
			{
				foreach (var assembly in TypeHelpers.GetAllInternalAssemblies())
				{
					var types = assembly.Types()
						.ThatImplement<DomainEvent>();

					types.Should()
						.BeUnderNamespace("Wally.CleanArchitecture.Domain");
				}
			}
		}

		[Fact]
		public void Domain_AllClassessEndsWithEvent_ShouldInheritDomainEvent()
		{
			var types = TypeHelpers.GetAllInternalAssemblies()
				.SelectMany(a => a.Types());

			using (new AssertionScope())
			{
				foreach (var type in types.Where(x => x.Name.EndsWith("Event") && x.Name != nameof(DomainEvent)))
				{
					type.Should()
						.BeAssignableTo<DomainEvent>();
				}
			}
		}

		[Fact]
		public void Domain_AllClassessWhichInheritsDomainEvent_ShouldHasEventSuffix()
		{
			var types = TypeHelpers.GetAllInternalAssemblies()
				.SelectMany(a => a.Types());

			using (new AssertionScope(new AssertionStrategy()))
			{
				foreach (var type in types.ThatImplement<DomainEvent>())
				{
					type.Name.Should()
						.EndWith("Event");
				}
			}
		}

		[Fact]
		public void Domain_DomainEvent_ShouldNotExposeSetter()
		{
			using (new AssertionScope(new AssertionStrategy()))
			{
				foreach (var assembly in TypeHelpers.GetAllInternalAssemblies())
				{
					var types = assembly.Types()
						.ThatImplement<DomainEvent>();

					types.Properties()
						.Should()
						.NotBeWritable("Request should be immutable");
				}
			}
		}
	}
}
