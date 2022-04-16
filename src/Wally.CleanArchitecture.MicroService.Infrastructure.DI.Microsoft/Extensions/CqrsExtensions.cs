using MediatR;

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
		services.AddMediatR(typeof(GetUserQuery));

		services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LogBehavior<,>));
		services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
		services.AddTransient(typeof(IPipelineBehavior<,>), typeof(DomainEventHandlerBehavior<,>));
		services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CommandHandlerValidatorBehavior<,>));
		services.AddTransient(typeof(IPipelineBehavior<,>), typeof(QueryHandlerValidatorBehavior<,>));

		services.Scan(
			a => a.FromAssemblyOf<UserCreatedDomainEventHandler>()
				.AddClasses(c => c.AssignableTo(typeof(IDomainEventHandler<>)))
				.AsImplementedInterfaces()
				.WithScopedLifetime());

		return services;
	}
}
