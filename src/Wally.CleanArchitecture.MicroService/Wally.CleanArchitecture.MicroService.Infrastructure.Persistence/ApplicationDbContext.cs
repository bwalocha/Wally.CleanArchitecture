using System;
using System.Linq;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

using Wally.CleanArchitecture.MicroService.Domain.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.Persistence;

public sealed class ApplicationDbContext : DbContext
{
	private const string RowVersion = nameof(RowVersion);

	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
		: base(options)
	{
		ChangeTracker.LazyLoadingEnabled = false;
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		// modelBuilder.HasDefaultSchema("users"); // TODO: consider to set DB Schema

		ConfigureProperties(modelBuilder);
		ConfigureStronglyTypedId(modelBuilder);
		ConfigureIdentityProperties(modelBuilder);

		// ConfigureConcurrencyTokens(modelBuilder); // TODO: Fix
	}

	private void ConfigureProperties(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfigurationsFromAssembly(
			GetType()
				.Assembly,
			type => type.Namespace!.StartsWith(
				GetType()
					.Namespace!));
	}

	private static void ConfigureIdentityProperties(ModelBuilder modelBuilder)
	{
		var allEntities = modelBuilder.Model.GetEntityTypes();
		foreach (var entity in allEntities)
		{
			const string idPropertyName = "Id"; // nameof(AggregateRoot<,>.Id); // TODO: "Id"
			var idProperty = entity.FindProperty(idPropertyName);
			if (idProperty != null)
			{
				idProperty.ValueGenerated = ValueGenerated.Never;
			}
		}
	}

	private static void ConfigureConcurrencyTokens(ModelBuilder modelBuilder)
	{
		var allEntities = modelBuilder.Model.GetEntityTypes();
		foreach (var entity in allEntities.Where(a => a.ClrType.IsSubclassOf(typeof(AggregateRoot<,>)))
					.Where(a => string.IsNullOrEmpty(a.GetViewName())))
		{
			var property = entity.AddProperty(RowVersion, typeof(DateTime));
			property.IsConcurrencyToken = true;
			property.ValueGenerated = ValueGenerated.OnAddOrUpdate;
		}
	}

	/// <summary>
	///     Configure the <see cref="ModelBuilder" /> to use the
	///     <see cref="StronglyTypedIdConverter{TStronglyTypedId,TValue}" />.
	/// </summary>
	/// <param name="modelBuilder">The ModelBuilder</param>
	public static void ConfigureStronglyTypedId(ModelBuilder modelBuilder)
	{
		var allEntities = modelBuilder.Model.GetEntityTypes();
		foreach (var entity in allEntities.Where(a => InheritsGenericClass(a.ClrType, typeof(Entity<,>)))
					.Where(a => string.IsNullOrEmpty(a.GetViewName()))
					.ToArray())
		{
			var entityBuilder = modelBuilder.Entity(entity.ClrType);
			entityBuilder.UseStronglyTypedId();
		}
	}

	private static bool InheritsGenericClass(Type type, Type classType)
	{
		if (!classType.IsClass)
		{
			throw new ArgumentException($"Parameter '{nameof(classType)}' is not a Class");
		}

		while (type != null && type != typeof(object))
		{
			var current = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
			if (classType == current)
			{
				return true;
			}

			if (type.BaseType == null)
			{
				break;
			}

			type = type.BaseType;
		}

		return false;
	}
}
