using MassTransit;
using Microsoft.EntityFrameworkCore;
using TickerQ.EntityFrameworkCore.Configurations;
using TickerQ.Utilities.Entities;
using Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.Extensions;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.Persistence;

public sealed class ApplicationDbContext : DbContext
{
	public const string SchemaName = "MicroService";

	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
		: base(options)
	{
		ChangeTracker.LazyLoadingEnabled = false;
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder
			.HasDefaultSchema(SchemaName)
			.ApplyMappings<IInfrastructurePersistenceAssemblyMarker>()
			.ApplyEnumConvention()
			.ApplyStronglyTypedId()
			.ApplyOptimisticConcurrency()
			.ApplySoftDelete()
			.ApplyTemporal();

		modelBuilder.AddInboxStateEntity();
		modelBuilder.AddOutboxMessageEntity();
		modelBuilder.AddOutboxStateEntity();
		
		modelBuilder.ApplyConfiguration(new TimeTickerConfigurations<TimeTickerEntity>(SchemaName));
		modelBuilder.ApplyConfiguration(new CronTickerConfigurations<CronTickerEntity>(SchemaName));
		modelBuilder.ApplyConfiguration(new CronTickerOccurrenceConfigurations<CronTickerEntity>(SchemaName));
	}
}
