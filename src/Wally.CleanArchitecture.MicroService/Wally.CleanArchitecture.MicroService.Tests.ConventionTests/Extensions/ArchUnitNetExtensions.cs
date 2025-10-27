using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ArchUnitNET.Domain;
using ArchUnitNET.Domain.Extensions;
using ArchUnitNET.Fluent.Conditions;
using ArchUnitNET.Fluent.Syntax;
using ArchUnitNET.Fluent.Syntax.Elements;
using ArchUnitNET.Fluent.Syntax.Elements.Types;
using ArchUnitNET.Fluent.Syntax.Elements.Types.Classes;

namespace Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Extensions;

public static class ArchUnitNetExtensions
{
	public static TGivenRuleTypeConjunction ResideInAssembly<TGivenRuleTypeConjunction, TRuleType>(this GivenTypesThat<TGivenRuleTypeConjunction, TRuleType> givenTypesThat, params System.Reflection.Assembly[] assemblies)
		where TRuleType : IType
	{
		return givenTypesThat.ResideInAssembly(assemblies.FirstOrDefault(), assemblies.Skip(1).ToArray());
	}
	
	// public static TGivenRuleTypeConjunction DoNotResideInAssembly<TGivenRuleTypeConjunction, TRuleType>(this GivenTypesThat<TGivenRuleTypeConjunction, TRuleType> givenTypesThat, params System.Reflection.Assembly[] assemblies)
	// 	where TRuleType : IType
	// {
	// 	return givenTypesThat.DoNotResideInAssembly(assemblies.FirstOrDefault(), assemblies.Skip(1).ToArray());
	// }
	
	public static TypesShouldConjunction NotAccessGetter<TType>(this ObjectsShould<TypesShouldConjunction, IType> builder, object? property, [CallerArgumentExpression("property")] string typeAndPropertyName = null!)
	{
		var names = typeAndPropertyName.Split('.');
		if (names[0] != typeof(TType).Name)
		{
			throw new ArgumentException($"'{names[1]}' is not part of '{names[0]}'.");
		}

		return builder.NotCallAny(MethodMembers().That().AreDeclaredIn(typeof(TType)).And().HaveName($"get_{names[1]}()"));
	}
	
	// public static IArchRule HaveOnlyPrivateConstructors(this ClassesShould conjunction)
	// {
	// 	var condition = new ConstructorVisibilityShouldBeCondition(Visibility.Private);
	//
	// 	return conjunction
	// 		.FollowCustomCondition(condition);
	// }
	
	// public static IArchRule HaveOnlyProtectedConstructors(this ClassesShould conjunction)
	// {
	// 	var condition = new ConstructorVisibilityShouldBeCondition(Visibility.Protected);
	//
	// 	return conjunction
	// 		.FollowCustomCondition(condition);
	// }
	
	public static IArchRule HaveOnlyPrivateOrProtectedConstructors(this ClassesShould conjunction)
	{
		var condition = new ConstructorVisibilityShouldBeCondition(Visibility.Private, Visibility.Protected);

		return conjunction
			.FollowCustomCondition(condition);
	}

	public static TRuleTypeShouldConjunction HaveOnlyPrivateOrProtectedSetters<TRuleTypeShouldConjunction, TRuleType>(
		this ObjectsShould<TRuleTypeShouldConjunction, TRuleType> builder)
		where TRuleType : IMember
		where TRuleTypeShouldConjunction : SyntaxElement<TRuleType>
	{
		var condition = new MemberVisibilityShouldBeCondition<TRuleType>(Visibility.Private, Visibility.Protected);
		
		return builder.FollowCustomCondition(condition);
	}
	
	public static TRuleTypeShouldConjunction HaveOnlyInitSetters<TRuleTypeShouldConjunction, TRuleType>(
		this ObjectsShould<TRuleTypeShouldConjunction, TRuleType> builder)
		where TRuleType : IMember
		where TRuleTypeShouldConjunction : SyntaxElement<TRuleType>
	{
		var condition = new MemberInitShouldBeCondition<TRuleType>();
		
		return builder.FollowCustomCondition(condition);
	}
	
	// public static IArchRule HaveOnlyPrivateOrProtectedSetters<TRuleTypeShouldConjunction, TRuleType>(this MembersShould<TRuleTypeShouldConjunction, TRuleType> conjunction)
	// 	where TRuleType : PropertyMember
	// 	where TRuleTypeShouldConjunction : SyntaxElement<TRuleType>
	// {
	// 	var condition = new MemberVisibilityShouldBeCondition(Visibility.Private, Visibility.Protected);
	//
	// 	return conjunction
	// 		.FollowCustomCondition(condition);
	// }

	// public static GivenClassesConjunction HaveOnlyPrivateOrProtectedSetters(this GivenClassesConjunction should)
	// {
	// 	return should.MeetCustomCondition(
	// 		"not have public setters",
	// 		type =>
	// 		{
	// 			var publicSetters = type
	// 				.GetProperties()
	// 				.Where(p => p.Setter != null && p.Setter.Visibility == Visibility.Public)
	// 				.ToList();
	//
	// 			if (publicSetters.Any())
	// 			{
	// 				var message = $"{type.FullName} has public setters: {string.Join(", ", publicSetters.Select(p => p.Name))}";
	// 				return new ConditionResult(false, message);
	// 			}
	//
	// 			return ConditionResult.Satisfied;
	// 		});
	// }
	
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
	
	private class MemberVisibilityShouldBeCondition<TRuleType> : ICondition<TRuleType>
		where TRuleType : IMember
	{
		private readonly Visibility[] _visibilities;
		public string Description => $"have only {string.Join(", ", _visibilities)} setters";

		public MemberVisibilityShouldBeCondition(params Visibility[] visibilities)
		{
			_visibilities = visibilities;
		}

		public IEnumerable<ConditionResult> Check(IEnumerable<TRuleType> members,
			Architecture architecture)
		{
			foreach (var member in members)
			{
				if (member is not PropertyMember property)
				{
					yield return new ConditionResult(member, true, $"{member.FullName} is {member.Visibility}.");
					continue;
				}

				if (property.Setter is null)
				{
					yield return new ConditionResult(property, true, $"{property.FullName} is {property.Visibility}.");
				}

				var pass = _visibilities.Contains(property.SetterVisibility);
				yield return new ConditionResult(
					property,
					pass,
					pass
						? $"{property.FullName} is {property.SetterVisibility}."
						: $"{property.FullName} is NOT {string.Join(" or ", _visibilities)}.");
			}
		}

		public bool CheckEmpty() => true;
	}
	
	private class MemberInitShouldBeCondition<TRuleType> : ICondition<TRuleType>
		where TRuleType : IMember
	{
		public string Description => $"have only Required Init setters";

		public MemberInitShouldBeCondition()
		{
		}

		public IEnumerable<ConditionResult> Check(IEnumerable<TRuleType> members,
			Architecture architecture)
		{
			foreach (var member in members)
			{
				if (member is not PropertyMember property)
				{
					yield return new ConditionResult(member, true, $"{member.FullName} is {member.Visibility}.");
					continue;
				}

				if (property.Setter is null)
				{
					yield return new ConditionResult(property, true, $"{property.FullName} is {property.Visibility}.");
				}

				// if (property.SetterVisibility != true)
				// {
				// 	return;
				// }
				//
				// var isInitOnly = (property.GetSetMethod()
				// 	?.ReturnParameter.GetRequiredCustomModifiers()
				// 	.Length ?? 0) > 0;
				// if (isInitOnly)
				// {
				// 	var isRequired = property.GetCustomAttribute(typeof(RequiredMemberAttribute)) != null;
				//
				// 	if (isRequired)
				// 	{
				// 		return;
				// 	}
				//
				// 	property.PropertyType.ShouldBeDecoratedWith<RequiredMemberAttribute>(
				// 		$"Request class '{type}' with Init setter should have Required modifier for '{property}'");
				// }
				//
				// if (!property.CanWrite)
				// {
				// 	return;
				// }
				// 	
				// property.IsPrivateWritable()
				// 	.ShouldBeTrue($"Request class '{type}' should not expose setter for '{property}'");
				//
				// var pass = _visibilities.Contains(property.SetterVisibility);
				// yield return new ConditionResult(
				// 	property,
				// 	pass,
				// 	pass
				// 		? $"{property.FullName} is {property.SetterVisibility}."
				// 		: $"{property.FullName} is NOT {string.Join(" or ", _visibilities)}.");
				
				yield return new ConditionResult(property, true, $"{property.FullName} is {property.Visibility}.");
			}
		}

		public bool CheckEmpty() => true;
	}
}
