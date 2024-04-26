using System;
using System.Linq;
using System.Linq.Expressions;
using MassTransit.Internals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Wally.CleanArchitecture.MicroService.Domain.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.Extensions;

public static class ModelBuilderExtensions
{
	public static ModelBuilder ApplyMappings<TInfrastructurePersistenceAssemblyMarker>(this ModelBuilder modelBuilder)
	{
		return modelBuilder.ApplyConfigurationsFromAssembly(typeof(TInfrastructurePersistenceAssemblyMarker).Assembly);
	}

	/// <summary>
	///     Configure the <see cref="ModelBuilder" /> to use the
	///     <see cref="StronglyTypedIdConverter{TStronglyTypedId,TValue}" />.
	/// </summary>
	/// <param name="modelBuilder">The ModelBuilder</param>
	public static ModelBuilder ApplyStronglyTypedId(this ModelBuilder modelBuilder)
	{
		var allEntities = modelBuilder.Model.GetEntityTypes();
		foreach (var entity in allEntities
					.Where(a => a.ClrType.HasInterface(typeof(IEntity)))
					.Where(a => string.IsNullOrEmpty(a.GetViewName()))
					.ToArray())
		{
			var entityBuilder = modelBuilder.Entity(entity.ClrType);
			entityBuilder.UseStronglyTypedId();
		}

		return modelBuilder;
	}

	public static ModelBuilder ApplySoftDelete(this ModelBuilder modelBuilder)
	{
		var allEntities = modelBuilder.Model.GetEntityTypes();
		foreach (var entity in allEntities
					.Where(a => a.ClrType.HasInterface(typeof(ISoftDeletable)))
					.Where(a => string.IsNullOrEmpty(a.GetViewName()))
					.Select(a => a.ClrType)
					.ToArray())
		{
			var entityBuilder = modelBuilder.Entity(entity);

			// TBD: https://learn.microsoft.com/en-us/ef/core/modeling/indexes?tabs=data-annotations#index-filter
			Expression<Func<ISoftDeletable, bool>> expression = a => !a.IsDeleted;
			var newParam = Expression.Parameter(entity);
			var newBody = ReplacingExpressionVisitor.Replace(expression.Parameters.Single(), newParam, expression.Body);
			entityBuilder.HasQueryFilter(Expression.Lambda(newBody, newParam));
		}

		return modelBuilder;
	}
}
