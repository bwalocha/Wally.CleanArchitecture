using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Wally.Lib.DDD.Abstractions.Commands;
using Wally.Lib.DDD.Abstractions.DomainEvents;
using Wally.Lib.DDD.Abstractions.DomainModels;

namespace Wally.CleanArchitecture.PipelineBehaviours;

public class DomainEventHandlerBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : ICommand, IRequest<TResponse>
{
	private readonly DbContext _dbContext;
	private readonly IServiceProvider _serviceProvider;

	public DomainEventHandlerBehavior(DbContext dbContext, IServiceProvider serviceProvider)
	{
		_dbContext = dbContext;
		_serviceProvider = serviceProvider;
	}

	public async Task<TResponse> Handle(
		TRequest request,
		CancellationToken cancellationToken,
		RequestHandlerDelegate<TResponse> next)
	{
		var response = await next();

		var domainEntities = _dbContext.ChangeTracker.Entries<AggregateRoot>()
			.Where(
				e => e.Entity.GetDomainEvents()
					.Any())
			.ToList();

		var domainEvents = domainEntities.SelectMany(x => x.Entity.GetDomainEvents())
			.ToList();

		foreach (var domainEvent in domainEvents)
		{
			var domainEvenHandlerType = typeof(IDomainEventHandler<>);
			var domainEvenHandlerTypeWithGenericType = domainEvenHandlerType.MakeGenericType(domainEvent.GetType());

			foreach (dynamic? service in _serviceProvider.GetServices(domainEvenHandlerTypeWithGenericType))
			{
				await service!.HandleAsync((dynamic)domainEvent, cancellationToken);
			}
		}

		return response;
	}
}
