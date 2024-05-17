using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Wally.CleanArchitecture.MicroService.Domain.Abstractions;
using Wally.Lib.DDD.Abstractions.Commands;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.PipelineBehaviours;

public class SoftDeleteBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : ICommand, IRequest<TResponse>
{
	private readonly IDateTimeProvider _dateTimeProvider;
	private readonly DbContext _dbContext;
	private readonly IUserProvider _userProvider;
	
	public SoftDeleteBehavior(
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
		
		UpdateSoftDeleteMetadata(_dbContext.ChangeTracker.Entries<ISoftDeletable>());
		
		return response;
	}
	
	private void UpdateSoftDeleteMetadata(IEnumerable<EntityEntry> entries)
	{
		var now = _dateTimeProvider.GetDateTime();
		var userId = _userProvider.GetCurrentUserId();
		
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
