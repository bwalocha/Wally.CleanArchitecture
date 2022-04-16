using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Wally.CleanArchitecture.MicroService.Messaging.Consumers;
using Wally.Lib.ServiceBus.Abstractions;
using Wally.Lib.ServiceBus.DI.Microsoft;
using Wally.Lib.ServiceBus.RabbitMQ;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Extensions;

public static class MessagingExtensions
{
	public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddPublisher();

		services.Scan(
			a => a.FromAssemblyOf<UserCreatedConsumer>()
				.AddClasses(c => c.AssignableTo(typeof(Consumer<>)))
				.AsImplementedInterfaces()
				.WithScopedLifetime());

		services.AddSingleton(
			_ => Factory.Create(new Settings(configuration.GetConnectionString(Constants.ServiceBus))));
		services.AddServiceBus();

		return services;
	}

	public static IApplicationBuilder UseMessaging(this IApplicationBuilder app)
	{
		app.UseServiceBus();

		return app;
	}
}
