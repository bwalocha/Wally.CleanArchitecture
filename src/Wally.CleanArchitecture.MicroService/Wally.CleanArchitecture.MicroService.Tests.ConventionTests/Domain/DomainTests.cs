using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Wally.CleanArchitecture.MicroService.Domain.Abstractions;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Extensions;

namespace Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Domain;

public class DomainTests
{
	[Fact]
	public void Domain_Constructor_ShouldBePrivateOrProtected()
	{
		// Arrange
		IArchRule rule =
			Classes()
				.That()
				.Are(Configuration.Assemblies.Domain.GetAllTypes())
				.And()
				.AreNotAbstract()
				.And()
				.AreNotAssignableTo(typeof(IStronglyTypedId<>))
				.And()
				.AreNotAssignableTo(typeof(TypeConverter))
				.And()
				.AreNotAssignableTo(typeof(ValueObject<>))
				.And()
				.AreNotAssignableTo(typeof(DomainEvent))
				.And()
				.AreNotAssignableTo(typeof(DomainException))
				.As("Domain classes")
				.Should()
				.HaveOnlyPrivateOrProtectedConstructors();
		
		// Act
		
		// Assert
		rule.Check(Configuration.Architecture);
	}
	
	[Fact]
	public void Domain_StronglyTypedId_ShouldBeSealed()
	{
		// Arrange
		IArchRule rule =
			Classes()
				.That()
				.Are(Configuration.Assemblies.Domain.GetAllTypes())
				.And()
				.AreNotAbstract()
				.And()
				.AreAssignableTo(typeof(IStronglyTypedId<>))
				.As("StronglyTypedId classes")
				.Should()
				.BeSealed();
		
		// Act
		
		// Assert
		rule.Check(Configuration.Architecture);
	}

	[Fact(Skip = "TODO: update")]
	public void Domain_Property_ShouldNotExposeSetter()
	{
		// Arrange
		IArchRule rule =
			Classes()
				.That()
				.Are(Configuration.Assemblies.Domain.GetAllTypes())
				// .And()
				// .AreNotAbstract()
				// .And()
				// .AreAssignableTo(typeof(IStronglyTypedId<>))
				.As("Domain classes")
				.Should()
				.BeImmutable();
		
		// Act
		
		// Assert
		rule.Check(Configuration.Architecture);
		
		
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.InheritsGenericClass(typeof(Entity<,>)));

		types.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in types)
			{
				foreach (var property in type.GetProperties())
				{
					if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType) &&
						property.PropertyType != typeof(string))
					{
						property.GetSetMethod()
							.ShouldBeNull($"Entity '{type}' and property '{property}' should be immutable");
					}
					else if (property.CanWrite)
					{
						property.IsPrivateWritable()
							.ShouldBeTrue($"Entity '{type}' and property '{property}' should be immutable");
					}
				}
			}
		});
	}

	[Fact(Skip = "Not implemented yet")]
	public void Domain_AggregateRootAndEntity_ShouldNotExposeWritableCollection()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.InheritsGenericClass(typeof(Entity<,>)));

		types.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in types)
			{
				foreach (var property in type.GetProperties())
				{
					if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType) &&
						property.PropertyType != typeof(string))
					{
						property.PropertyType.ImplementsGenericInterface(typeof(IReadOnlyCollection<>))
							.ShouldBeTrue($"Entity '{type}' should not expose writable collection '{property}'");
					}
				}
			}
		});
	}

	[Fact(Skip = "Not implemented yet")]
	public void Domain_ValueObject_ShouldNotExposeSetter()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.InheritsGenericClass(typeof(ValueObject<>)));

		types.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in types)
			{
				foreach (var property in type.GetProperties()
							.Where(a => a.CanWrite))
				{
					property.IsPrivateWritable()
						.ShouldBeTrue($"ValueObject '{type}' should not expose setter '{property}'");
				}
			}
		});
	}

	[Fact(Skip = "Not implemented yet")]
	public void Domain_StronglyTypedId_ShouldHaveExplicitOperator()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.ImplementsGenericInterface(typeof(IStronglyTypedId<,>)))
			.Where(a => !a.IsGenericType);

		types.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in types)
			{
				var explicitOperator = type.GetMethod(
					"op_Explicit",
					BindingFlags.Public | BindingFlags.Static,
					null,
					[type,],
					null);

				explicitOperator.ShouldNotBeNull($"StronglyTypedId '{type}' should have explicit operator");
			}
		});
	}
}
