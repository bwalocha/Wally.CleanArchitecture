using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Wally.CleanArchitecture.MicroService.Application.Abstractions;
using Wally.CleanArchitecture.MicroService.Domain.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.PipelineBehaviours;

public class SoftDeleteBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : Application.Abstractions.ICommand<TResponse>
{
	private readonly DbContext _dbContext;
	private readonly TimeProvider _timeProvider;
	private readonly IRequestContext _requestContext;

	public SoftDeleteBehavior(
		DbContext dbContext,
		IRequestContext requestContext,
		TimeProvider timeProvider)
	{
		_dbContext = dbContext;
		_requestContext = requestContext;
		_timeProvider = timeProvider;
	}
	
	public async ValueTask<TResponse> Handle(TRequest message, MessageHandlerDelegate<TRequest, TResponse> next, CancellationToken cancellationToken)
	{
		var response = await next(message, cancellationToken);
		var entries = _dbContext.ChangeTracker
			.Entries<ISoftDeletable>()
			.Where(e => e.State == EntityState.Deleted);
		UpdateSoftDeleteMetadata(_dbContext, entries, _requestContext, _timeProvider);

		return response;
	}

	private static void UpdateSoftDeleteMetadata(
		DbContext dbContext,
		IEnumerable<EntityEntry> entries,
		IRequestContext requestContext,
		TimeProvider timeProvider)
	{
		var utcNow = timeProvider.GetUtcNow();
		var userId = requestContext.UserId;

		foreach (var entry in entries)
		{
			entry.State = EntityState.Modified;
			entry.CurrentValues.SetValues(
				new Dictionary<string, object>
				{
					{
						nameof(ISoftDeletable.IsDeleted), true
					},
					{
						nameof(ISoftDeletable.DeletedAt), utcNow
					},
					{
						nameof(ISoftDeletable.DeletedById), userId
					},
				});
			HandleDependencies(dbContext, entry);
		}
	}
	
	private static void HandleDependencies(DbContext context, EntityEntry entry)
	{
		// https://www.youtube.com/watch?v=7teQpp5V4Vs
		var ownedReferencedEntries = entry.References
			.Where(x => x.TargetEntry != null)
			.Select(x => x.TargetEntry!)
			.Where(x => x.State == EntityState.Deleted && x.Metadata.IsOwned());

		foreach (var ownedEntry in ownedReferencedEntries)
		{
			ownedEntry.State = EntityState.Unchanged;
			HandleDependencies(context, ownedEntry);
		}

		var ownedCollectionEntries = entry.Collections
			.Where(x => x is { IsLoaded: true, CurrentValue: not null, })
			.SelectMany(x => x.CurrentValue!.Cast<object>().Select(context.Entry))
			.Where(x => x.State == EntityState.Deleted && x.Metadata.IsOwned());

		foreach (var ownedEntry in ownedCollectionEntries)
		{
			ownedEntry.State = EntityState.Unchanged;
			HandleDependencies(context, ownedEntry);
		}
	}
}
