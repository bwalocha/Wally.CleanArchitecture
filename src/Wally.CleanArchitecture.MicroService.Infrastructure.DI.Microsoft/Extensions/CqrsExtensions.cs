using Microsoft.Extensions.DependencyInjection;

using Wally.CleanArchitecture.MicroService.Application;
using Wally.CleanArchitecture.MicroService.Infrastructure.PipelineBehaviours;
using Wally.Lib.DDD.Abstractions.DomainEvents;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Extensions;

public static class CqrsExtensions
{
	public static IServiceCollection AddCqrs(this IServiceCollection services)
	{
		services.AddMediatR(
			a =>
			{
				a.RegisterServicesFromAssemblyContaining<IApplicationAssemblyMarker>();

				a.AddOpenBehavior(typeof(LogBehavior<,>));
				a.AddOpenBehavior(typeof(TransactionBehavior<,>));
				a.AddOpenBehavior(typeof(UpdateMetadataHandlerBehavior<,>));
				a.AddOpenBehavior(typeof(DomainEventHandlerBehavior<,>));
				a.AddOpenBehavior(typeof(UpdateMetadataHandlerBehavior<,>));

				// a.AddOpenBehavior(typeof(CommandHandlerValidatorBehavior<,>));
				a.AddOpenBehavior(typeof(CommandHandlerValidatorsBehavior<,>));
				a.AddOpenBehavior(typeof(QueryHandlerValidatorBehavior<,>));
			});

		services.Scan(
			a => a.FromAssemblyOf<IApplicationAssemblyMarker>()
				.AddClasses(c => c.AssignableTo(typeof(IDomainEventHandler<>)))
				.AsImplementedInterfaces()
				.WithScopedLifetime());

		return services;
	}
}
