using System.Linq;
using MassTransit.Internals;
using Microsoft.EntityFrameworkCore;
using Wally.CleanArchitecture.MicroService.Domain.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.Persistence;

public sealed class ApplicationDbContext : DbContext
{
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
		foreach (var entity in allEntities.Where(a => a.ClrType.HasInterface(typeof(IEntity)))
					.Where(a => string.IsNullOrEmpty(a.GetViewName()))
					.ToArray())
		{
			var entityBuilder = modelBuilder.Entity(entity.ClrType);
			entityBuilder.UseStronglyTypedId();
		}
	}
}
