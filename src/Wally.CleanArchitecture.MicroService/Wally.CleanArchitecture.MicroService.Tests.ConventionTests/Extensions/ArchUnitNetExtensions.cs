using System.Collections.Generic;
using System.Linq;
using ArchUnitNET.Domain;
using ArchUnitNET.Domain.Extensions;
using ArchUnitNET.Fluent.Conditions;
using ArchUnitNET.Fluent.Syntax.Elements.Types.Classes;

namespace Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Extensions;

public static class ArchUnitNetExtensions
{
	public static IArchRule HaveOnlyPrivateConstructors(this ClassesShould conjunction)
	{
		var condition = new ConstructorVisibilityShouldBeCondition(Visibility.Private);

		return conjunction
			.FollowCustomCondition(condition);
	}
	
	public static IArchRule HaveOnlyProtectedConstructors(this ClassesShould conjunction)
	{
		var condition = new ConstructorVisibilityShouldBeCondition(Visibility.Protected);

		return conjunction
			.FollowCustomCondition(condition);
	}
	
	public static IArchRule HaveOnlyPrivateOrProtectedConstructors(this ClassesShould conjunction)
	{
		var condition = new ConstructorVisibilityShouldBeCondition(Visibility.Private, Visibility.Protected);

		return conjunction
			.FollowCustomCondition(condition);
	}
	
	private class ConstructorVisibilityShouldBeCondition : ICondition<Class>
	{
		private readonly Visibility[] _visibilities;
		public string Description => $"have only {string.Join(", ", _visibilities)} constructors";

		public ConstructorVisibilityShouldBeCondition(params Visibility[] visibilities)
		{
			_visibilities = visibilities;
		}

		public IEnumerable<ConditionResult> Check(IEnumerable<Class> classes, Architecture architecture)
		{
			foreach (var @class in classes)
			{
				foreach (var constructor in @class.GetConstructors())
				{
					bool pass = _visibilities.Contains(constructor.Visibility);
					yield return new ConditionResult(
						@class,
						pass,
						pass
							? $"{constructor.FullName} is {constructor.Visibility}."
							: $"{constructor.FullName} is NOT {string.Join(" or ", _visibilities)}.");
				}
			}
		}

		public bool CheckEmpty() => true;
	}
}
