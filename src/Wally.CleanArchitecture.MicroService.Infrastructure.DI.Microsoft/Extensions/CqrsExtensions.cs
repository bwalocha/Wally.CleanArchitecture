using Microsoft.Extensions.DependencyInjection;

using Wally.CleanArchitecture.MicroService.Application.Users;
using Wally.CleanArchitecture.MicroService.Application.Users.Queries;
using Wally.CleanArchitecture.MicroService.PipelineBehaviours;
using Wally.Lib.DDD.Abstractions.DomainEvents;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Extensions;

public static class CqrsExtensions
{
	public static IServiceCollection AddCqrs(this IServiceCollection services)
	{
		services.AddMediatR(a =>
		{
			a.RegisterServicesFromAssemblyContaining<GetUserQuery>();
			
			a.AddOpenBehavior(typeof(LogBehavior<,>));
			a.AddOpenBehavior(typeof(TransactionBehavior<,>));
			a.AddOpenBehavior(typeof(DomainEventHandlerBehavior<,>));
			a.AddOpenBehavior(typeof(CommandHandlerValidatorBehavior<,>));
			a.AddOpenBehavior(typeof(QueryHandlerValidatorBehavior<,>));
		});

		services.Scan(
			a => a.FromAssemblyOf<UserCreatedDomainEventHandler>()
				.AddClasses(c => c.AssignableTo(typeof(IDomainEventHandler<>)))
				.AsImplementedInterfaces()
				.WithScopedLifetime());

		return services;
	}
}
