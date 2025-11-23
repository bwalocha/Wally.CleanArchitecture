using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Wally.CleanArchitecture.MicroService.Application.Abstractions;
using Wally.CleanArchitecture.MicroService.Domain.Abstractions;
using Wally.CleanArchitecture.MicroService.Infrastructure.Persistence;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.PipelineBehaviours;

public class DomainEventHandlerBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : Application.Abstractions.ICommand<TResponse>
{
	private readonly DbContext _dbContext;
	private readonly IServiceProvider _serviceProvider;

	public DomainEventHandlerBehavior(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
	{
		_dbContext = dbContext;
		_serviceProvider = serviceProvider;
	}

	public async ValueTask<TResponse> Handle(TRequest message, MessageHandlerDelegate<TRequest, TResponse> next,
		CancellationToken cancellationToken)
	{
		var response = await next(message, cancellationToken);

		var domainEntities = _dbContext.ChangeTracker.Entries<IEntity>()
			.Where(a => a.Entity.GetDomainEvents()
				.Count != 0)
			.ToList();

		var domainEvents = domainEntities.SelectMany(x => x.Entity.GetDomainEvents())
			.ToList();

		await _dbContext.SaveChangesAsync(cancellationToken);

		foreach (var domainEvent in domainEvents)
		{
			// TODO: consider using cache: dictionary
			var domainEvenHandlerType = typeof(IDomainEventHandler<>);
			var domainEvenHandlerTypeWithGenericType = domainEvenHandlerType.MakeGenericType(domainEvent.GetType());

			foreach (dynamic? service in _serviceProvider.GetServices(domainEvenHandlerTypeWithGenericType))
			{
				await service!.HandleAsync((dynamic)domainEvent, cancellationToken);
			}

			domainEntities.Single(a => a.Entity
					.GetDomainEvents()
					.Contains(domainEvent))
				.Entity
				.RemoveDomainEvent(domainEvent);
		}

		// TODO: consider throw an Exception if any DomainEventHandler produced another DomainEvent

		return response;
	}
}
