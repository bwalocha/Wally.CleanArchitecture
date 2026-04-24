using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wally.CleanArchitecture.MicroService.Domain.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.SqlServer.Extensions;

public static class ModelBuilderExtensions
{
	// https://learn.microsoft.com/en-us/ef/core/providers/sql-server/temporal-tables
	// https://github.com/dotnet/efcore/issues/29303
	private const string TemporalPostfix = "_Temporal";
	private const string TemporalPeriodStart = "PeriodStart";
	private const string TemporalPeriodEnd = "PeriodEnd";
	
	public static EntityTypeBuilder<TModel> ConfigureTemporal<TModel>(this EntityTypeBuilder<TModel> builder, string? tableName = null)
		where TModel : class, ITemporal
		=> builder.ToTable(
			tableName ?? typeof(TModel).Name,
			a =>
			{
				a.IsTemporal(b =>
				{
					b.UseHistoryTable($"{tableName ?? typeof(TModel).Name}{TemporalPostfix}");
					b.HasPeriodStart(TemporalPeriodStart);
					b.HasPeriodEnd(TemporalPeriodEnd);
				});
				
				a.Property<DateTime>(TemporalPeriodStart).HasColumnName(TemporalPeriodStart);
				a.Property<DateTime>(TemporalPeriodEnd).HasColumnName(TemporalPeriodEnd);
			});

	public static OwnedNavigationBuilder<TModel, TDependent> ConfigureTemporal<TModel, TDependent>(this OwnedNavigationBuilder<TModel, TDependent> builder, string? tableName = null)
		where TModel : class, ITemporal
		where TDependent : class
		=> builder.ToTable(
			tableName ?? typeof(TModel).Name,
			a =>
			{
				a.IsTemporal(b =>
				{
					b.UseHistoryTable($"{tableName ?? typeof(TModel).Name}{TemporalPostfix}");
					b.HasPeriodStart(TemporalPeriodStart);
					b.HasPeriodEnd(TemporalPeriodEnd);
				});
				
				a.Property<DateTime>(TemporalPeriodStart).HasColumnName(TemporalPeriodStart);
				a.Property<DateTime>(TemporalPeriodEnd).HasColumnName(TemporalPeriodEnd);
			});
}
