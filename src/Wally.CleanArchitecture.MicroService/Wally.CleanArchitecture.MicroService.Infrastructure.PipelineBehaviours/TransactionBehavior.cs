using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.PipelineBehaviours;

public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : Application.Abstractions.ICommand<TResponse>
{
	private readonly DbContext _dbContext;

	public TransactionBehavior(DbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async ValueTask<TResponse> Handle(TRequest message, MessageHandlerDelegate<TRequest, TResponse> next,
		CancellationToken cancellationToken)
	{
		var strategy = _dbContext.Database.CreateExecutionStrategy();

		return await strategy.ExecuteInTransactionAsync<DbContext, TResponse>(
			state: _dbContext,
			operation: async (state, token) =>
			{
				var response = await next(message, token);
				await state.SaveChangesAsync(token);
				
				return response;
			},
			verifySucceeded: (_, _) => Task.FromResult(true),
			IsolationLevel.ReadCommitted,
			cancellationToken: cancellationToken);
	}
}
