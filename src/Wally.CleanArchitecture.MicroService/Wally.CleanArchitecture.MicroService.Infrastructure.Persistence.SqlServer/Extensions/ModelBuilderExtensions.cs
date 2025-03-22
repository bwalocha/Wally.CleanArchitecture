using System.Linq;
using Microsoft.EntityFrameworkCore;
using Wally.CleanArchitecture.MicroService.Domain.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.SqlServer.Extensions;

public static class ModelBuilderExtensions
{
	public static ModelBuilder ApplyChangeTracking(this ModelBuilder modelBuilder)
	{
		var allEntities = modelBuilder.Model.GetEntityTypes();
		foreach (var entity in allEntities
					.Where(a => typeof(ISoftDeletable)
						.IsAssignableFrom(a.ClrType)) // TODO: use different interface or apply for all entities
					.Where(a => string.IsNullOrEmpty(a.GetViewName()))
					.Select(a => a.ClrType)
					.ToArray())
		{
			var entityBuilder = modelBuilder.Entity(entity);

			// https://learn.microsoft.com/en-us/ef/core/providers/sql-server/temporal-tables
			entityBuilder.ToTable(
				entity.Name,
				a => a.IsTemporal(
					/*b =>
					{
						b.HasPeriodStart("ValidFrom");
						b.HasPeriodEnd("ValidTo");
						b.UseHistoryTable($"{entity.Name}_HistoricalData");
					}*/
				));
		}

		return modelBuilder;
	}
}
