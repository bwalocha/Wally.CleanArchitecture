using System.Threading;
using System.Threading.Tasks;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.PipelineBehaviours;

public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : Application.Abstractions.ICommand<TResponse>
{
	private readonly DbContext _dbContext;

	public TransactionBehavior(DbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async ValueTask<TResponse> Handle(TRequest message, MessageHandlerDelegate<TRequest, TResponse> next, CancellationToken cancellationToken)
	{
		await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

		try
		{
			var response = await next(message, cancellationToken);
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
