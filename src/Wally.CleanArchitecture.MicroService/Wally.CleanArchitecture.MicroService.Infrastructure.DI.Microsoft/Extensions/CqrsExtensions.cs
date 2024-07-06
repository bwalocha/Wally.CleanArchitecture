using MediatR;
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
			a => { a.RegisterServicesFromAssemblyContaining<IApplicationAssemblyMarker>(); });

		services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LogBehavior<,>));
		services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
		services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UpdateMetadataHandlerBehavior<,>));
		services.AddTransient(typeof(IPipelineBehavior<,>), typeof(SoftDeleteBehavior<,>));
		services.AddTransient(typeof(IPipelineBehavior<,>), typeof(DomainEventHandlerBehavior<,>));
		services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UpdateMetadataHandlerBehavior<,>));
		services.AddTransient(typeof(IPipelineBehavior<,>), typeof(SoftDeleteBehavior<,>));
		services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CommandHandlerValidatorBehavior<,>));
		services.AddTransient(typeof(IPipelineBehavior<,>), typeof(QueryHandlerValidatorBehavior<,>));

		services.Scan(
			a => a.FromAssemblyOf<IApplicationAssemblyMarker>()
				.AddClasses(c => c.AssignableTo(typeof(IDomainEventHandler<>)))
				.AsImplementedInterfaces()
				.WithScopedLifetime());

		return services;
	}
}
