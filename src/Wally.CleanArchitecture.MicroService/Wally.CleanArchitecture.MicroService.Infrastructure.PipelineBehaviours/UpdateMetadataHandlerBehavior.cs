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

public class UpdateMetadataHandlerBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : ICommand<TResponse>
{
	private readonly DbContext _dbContext;
	private readonly TimeProvider _timeProvider;
	private readonly IUserProvider _userProvider;

	public UpdateMetadataHandlerBehavior(
		DbContext dbContext,
		IUserProvider userProvider,
		TimeProvider timeProvider)
	{
		_dbContext = dbContext;
		_userProvider = userProvider;
		_timeProvider = timeProvider;
	}

	public async Task<TResponse> Handle(
		TRequest request,
		RequestHandlerDelegate<TResponse> next,
		CancellationToken cancellationToken)
	{
		var response = await next();

		UpdateAggregateMetadata(_dbContext.ChangeTracker.Entries<IAuditable>());

		return response;
	}

	private void UpdateAggregateMetadata(IEnumerable<EntityEntry> entries)
	{
		var now = _timeProvider.GetUtcNow();
		var userId = _userProvider.GetCurrentUserId();

		foreach (var entry in entries)
		{
			switch (entry.State)
			{
				case EntityState.Modified:
					entry.CurrentValues.SetValues(
						new Dictionary<string, object>
						{
							{
								nameof(IAggregateRoot.ModifiedAt), now
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
								nameof(IAggregateRoot.CreatedAt), now
							},
							{
								nameof(IAggregateRoot.CreatedById), userId
							},
						});
					break;
				default:
					continue;
			}
		}
	}
}
