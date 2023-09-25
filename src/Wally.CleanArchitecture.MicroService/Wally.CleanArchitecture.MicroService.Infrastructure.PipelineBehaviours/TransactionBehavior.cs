using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Microsoft.EntityFrameworkCore;

using Wally.Lib.DDD.Abstractions.Commands;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.PipelineBehaviours;

public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : ICommand, IRequest<TResponse>
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
			var response = await next();
			await _dbContext.SaveChangesAsync(cancellationToken);
			await transaction.CommitAsync(cancellationToken);

			return response;
		}
		catch
		{
			await transaction.RollbackAsync(cancellationToken);
			throw;
		}
	}
}
