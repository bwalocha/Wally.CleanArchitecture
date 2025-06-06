﻿using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Wally.CleanArchitecture.MicroService.Application.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.PipelineBehaviours;

public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : ICommand<TResponse>
{
	private readonly DbContext _dbContext;

	public TransactionBehavior(DbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task<TResponse> Handle(
		TRequest request,
		RequestHandlerDelegate<TResponse> next,
		CancellationToken cancellationToken)
	{
		await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

		try
		{
			var response = await next(cancellationToken);
			await _dbContext.SaveChangesAsync(cancellationToken);
			await transaction.CommitAsync(cancellationToken);

			return response;
		}
		catch
		{
			System.Diagnostics.Debugger.Break();
			await transaction.RollbackAsync(cancellationToken);

			throw;
		}
		finally
		{
			_dbContext.ChangeTracker.Clear();
		}
	}
}
