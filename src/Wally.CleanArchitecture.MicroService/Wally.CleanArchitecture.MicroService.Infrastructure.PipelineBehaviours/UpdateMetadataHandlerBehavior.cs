using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Wally.CleanArchitecture.MicroService.Application.Abstractions;
using Wally.CleanArchitecture.MicroService.Domain.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.PipelineBehaviours;

public class UpdateMetadataHandlerBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : Application.Abstractions.ICommand<TResponse>
{
	private readonly DbContext _dbContext;
	private readonly IRequestContext _requestContext;
	private readonly TimeProvider _timeProvider;

	public UpdateMetadataHandlerBehavior(
		DbContext dbContext,
		IRequestContext requestContext,
		TimeProvider timeProvider)
	{
		_dbContext = dbContext;
		_requestContext = requestContext;
		_timeProvider = timeProvider;
	}

	public async ValueTask<TResponse> Handle(TRequest message, MessageHandlerDelegate<TRequest, TResponse> next,
		CancellationToken cancellationToken)
	{
		var response = await next(message, cancellationToken);

		UpdateAggregateMetadata(_dbContext.ChangeTracker.Entries<IAuditable>());

		return response;
	}

	private void UpdateAggregateMetadata(IEnumerable<EntityEntry> entries)
	{
		var utcNow = _timeProvider.GetUtcNow();
		var userId = _requestContext.UserId;

		foreach (var entry in entries)
		{
			switch (entry.State)
			{
				case EntityState.Modified:
					entry.CurrentValues.SetValues(
						new Dictionary<string, object>
						{
							{
								nameof(IAggregateRoot.ModifiedAt), utcNow
							},
							{
								nameof(IAggregateRoot.ModifiedById), userId
							},
						});
					break;
				case EntityState.Added:
					entry.CurrentValues.SetValues(
						new Dictionary<string, object>
						{
							{
								nameof(IAggregateRoot.CreatedAt), utcNow
							},
							{
								nameof(IAggregateRoot.CreatedById), userId
							},
						});
					break;
				case EntityState.Detached:
				case EntityState.Unchanged:
				case EntityState.Deleted:
				default:
					continue;
			}
		}
	}
}
