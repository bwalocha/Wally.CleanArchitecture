using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Wally.CleanArchitecture.MicroService.Domain.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.Extensions;

public static class ModelBuilderExtensions
{
	private const string TemporalPostfix = "_Temporal";
	private const string TemporalValidFrom = "ValidFrom";
	private const string TemporalValidTo = "ValidTo";
	
	public static ModelBuilder ApplyMappings<TInfrastructurePersistenceAssemblyMarker>(this ModelBuilder modelBuilder)
	{
		return modelBuilder.ApplyConfigurationsFromAssembly(typeof(TInfrastructurePersistenceAssemblyMarker).Assembly);
	}

	public static ModelBuilder ApplyEnumConvention(this ModelBuilder modelBuilder)
	{
		var allEntities = modelBuilder.Model.GetEntityTypes();

		foreach (var property in allEntities
					.SelectMany(a => a.GetProperties())
					.Where(a => a.ClrType.IsEnum)
					.Where(a => !a.ClrType.IsDefined(typeof(FlagsAttribute), false)))
		{
			var converterType = typeof(EnumToStringConverter<>).MakeGenericType(property.ClrType);
			var converter = (ValueConverter)Activator.CreateInstance(converterType) !;
			property.SetValueConverter(converter);
		}

		return modelBuilder;
	}
	
	/// <summary>
	///     Configure the <see cref="ModelBuilder" /> to use the
	///     <see cref="StronglyTypedIdConverter{TStronglyTypedId,TValue}" />.
	/// </summary>
	/// <param name="modelBuilder">The ModelBuilder</param>
	public static ModelBuilder ApplyStronglyTypedId(this ModelBuilder modelBuilder)
	{
		foreach (var entityType in GetEntityTypes<IEntity>(modelBuilder))
		{
			var entityBuilder = modelBuilder.Entity(entityType);

			entityBuilder.UseStronglyTypedId();
		}

		return modelBuilder;
	}

	public static ModelBuilder ApplyOptimisticConcurrency(this ModelBuilder modelBuilder)
	{
		foreach (var entityType in GetEntityTypes<IAggregateRoot>(modelBuilder))
		{
			var entityBuilder = modelBuilder.Entity(entityType);

			// https://learn.microsoft.com/en-us/ef/core/saving/concurrency?tabs=fluent-api
			entityBuilder.Property(nameof(IAggregateRoot.ModifiedAt))
				.IsConcurrencyToken();
		}

		return modelBuilder;
	}

	public static ModelBuilder ApplySoftDelete(this ModelBuilder modelBuilder)
	{
		foreach (var entityType in GetEntityTypes<ISoftDeletable>(modelBuilder))
		{
			var entityBuilder = modelBuilder.Entity(entityType);

			// TBD: https://learn.microsoft.com/en-us/ef/core/modeling/indexes?tabs=data-annotations#index-filter
			Expression<Func<ISoftDeletable, bool>> expression = a => !a.IsDeleted;
			var newParam = Expression.Parameter(entityType);
			var newBody = ReplacingExpressionVisitor.Replace(expression.Parameters.Single(), newParam, expression.Body);
			entityBuilder.HasQueryFilter(Expression.Lambda(newBody, newParam));
		}

		return modelBuilder;
	}
	
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
						b.HasPeriodStart(nameof(TemporalValidFrom));
						b.HasPeriodEnd(nameof(TemporalValidTo));
					}));
		}

		return modelBuilder;
	}

	private static IEnumerable<Type> GetEntityTypes<TType>(this ModelBuilder modelBuilder)
	{
		var allEntities = modelBuilder.Model.GetEntityTypes();

		return allEntities
			.Where(a => typeof(TType).IsAssignableFrom(a.ClrType))
			// .Where(a => string.IsNullOrEmpty(a.GetViewName())) // TODO: Remove for VW Migrations
			.Select(a => a.ClrType);
	}
}
