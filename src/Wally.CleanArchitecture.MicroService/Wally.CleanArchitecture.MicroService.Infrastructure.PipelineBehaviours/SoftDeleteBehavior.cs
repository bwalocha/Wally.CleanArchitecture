using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Wally.CleanArchitecture.MicroService.Application.Abstractions;
using Wally.CleanArchitecture.MicroService.Domain.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.PipelineBehaviours;

public class SoftDeleteBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : ICommand<TResponse>
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

	public async Task<TResponse> Handle(
		TRequest request,
		RequestHandlerDelegate<TResponse> next,
		CancellationToken cancellationToken)
	{
		var response = await next(cancellationToken);

		UpdateSoftDeleteMetadata(_dbContext.ChangeTracker.Entries<ISoftDeletable>());

		return response;
	}

	private void UpdateSoftDeleteMetadata(IEnumerable<EntityEntry> entries)
	{
		var now = _timeProvider.GetUtcNow();
		var userId = _requestContext.UserId;

		foreach (var entry in entries)
		{
			switch (entry.State)
			{
				case EntityState.Deleted:
					entry.State = EntityState.Modified;
					entry.CurrentValues.SetValues(
						new Dictionary<string, object>
						{
							{
								nameof(ISoftDeletable.IsDeleted), true
							},
							{
								nameof(ISoftDeletable.DeletedAt), now
							},
							{
								nameof(ISoftDeletable.DeletedById), userId
							},
						});
					break;
				default:
					continue;
			}
		}
	}
}
