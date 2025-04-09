/*
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Wally.CleanArchitecture.MicroService.Domain.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.SqlServer.Extensions;

public static class ModelBuilderExtensions
{
	private const string TemporalPostfix = "_Temporal";
	
	public static ModelBuilder ApplyTemporal(this ModelBuilder modelBuilder)
	{
		var allEntities = modelBuilder.Model.GetEntityTypes();
		foreach (var entity in allEntities
					.Where(a => typeof(ITemporal)
						.IsAssignableFrom(a.ClrType))
					.Where(a => string.IsNullOrEmpty(a.GetViewName()))
					.Select(a => a.ClrType)
					.ToArray())
		{
			var entityBuilder = modelBuilder.Entity(entity);

			// https://learn.microsoft.com/en-us/ef/core/providers/sql-server/temporal-tables
			entityBuilder.ToTable(
				entity.Name,
				a => a.IsTemporal(
					b =>
					{
						b.UseHistoryTable($"{entity.Name}{TemporalPostfix}");
						b.HasPeriodStart(nameof(ITemporal.ValidFrom));
						b.HasPeriodEnd(nameof(ITemporal.ValidTo));
					}));
		}

		return modelBuilder;
	}
}
*/
