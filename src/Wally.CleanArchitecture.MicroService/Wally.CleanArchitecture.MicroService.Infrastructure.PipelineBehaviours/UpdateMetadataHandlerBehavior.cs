using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using Wally.CleanArchitecture.MicroService.Domain.Abstractions;
using Wally.Lib.DDD.Abstractions.Commands;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.PipelineBehaviours;

public class UpdateMetadataHandlerBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : ICommand, IRequest<TResponse>
{
	private readonly IDateTimeProvider _dateTimeProvider;
	private readonly DbContext _dbContext;
	private readonly IUserProvider _userProvider;

	public UpdateMetadataHandlerBehavior(
		DbContext dbContext,
		IUserProvider userProvider,
		IDateTimeProvider dateTimeProvider)
	{
		_dbContext = dbContext;
		_userProvider = userProvider;
		_dateTimeProvider = dateTimeProvider;
	}

	public async Task<TResponse> Handle(
		TRequest request,
		RequestHandlerDelegate<TResponse> next,
		CancellationToken cancellationToken)
	{
		var response = await next();

		UpdateAggregateMetadata(_dbContext.ChangeTracker.Entries<IAggregateRoot>());

		return response;
	}

	private void UpdateAggregateMetadata(IEnumerable<EntityEntry<IAggregateRoot>> entries)
	{
		var now = _dateTimeProvider.GetDateTime();

		foreach (var entry in entries)
		{
			switch (entry.State)
			{
				case EntityState.Modified:
					entry.CurrentValues.SetValues(
						new Dictionary<string, object>
						{
							{ nameof(IAggregateRoot.ModifiedAt), now },
							{ nameof(IAggregateRoot.ModifiedById), _userProvider.GetCurrentUserId() },
						});
					break;
				case EntityState.Added:
					entry.CurrentValues.SetValues(
						new Dictionary<string, object>
						{
							{ nameof(IAggregateRoot.CreatedAt), now },
							{ nameof(IAggregateRoot.CreatedById), _userProvider.GetCurrentUserId() },
						});
					break;
			}
		}
	}
}
