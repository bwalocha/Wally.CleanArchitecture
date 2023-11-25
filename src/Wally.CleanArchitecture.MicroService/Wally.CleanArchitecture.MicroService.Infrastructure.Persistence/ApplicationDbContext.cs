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
		modelBuilder.HasDefaultSchema("MicroService");

		ConfigureMappings(modelBuilder);
		ConfigureStronglyTypedId(modelBuilder);
	}

	private static void ConfigureMappings(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfigurationsFromAssembly(typeof(IInfrastructurePersistenceAssemblyMarker).Assembly);
	}

	/// <summary>
	///     Configure the <see cref="ModelBuilder" /> to use the
	///     <see cref="StronglyTypedIdConverter{TStronglyTypedId,TValue}" />.
	/// </summary>
	/// <param name="modelBuilder">The ModelBuilder</param>
	private static void ConfigureStronglyTypedId(ModelBuilder modelBuilder)
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
