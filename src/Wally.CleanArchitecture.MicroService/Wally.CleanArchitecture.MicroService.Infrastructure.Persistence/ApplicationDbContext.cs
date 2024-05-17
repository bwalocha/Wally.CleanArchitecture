using Microsoft.EntityFrameworkCore;
using Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.Extensions;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.Persistence;

public sealed class ApplicationDbContext : DbContext
{
	private const string DefaultSchema = "MicroService";
	
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
		: base(options)
	{
		ChangeTracker.LazyLoadingEnabled = false;
	}
	
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder
			.HasDefaultSchema(DefaultSchema)
			.ApplyMappings<IInfrastructurePersistenceAssemblyMarker>()
			.ApplyStronglyTypedId()
			.ApplySoftDelete();
	}
}
