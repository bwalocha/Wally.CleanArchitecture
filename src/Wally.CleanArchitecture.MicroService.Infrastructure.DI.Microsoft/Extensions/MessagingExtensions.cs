using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

using Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Models;
using Wally.CleanArchitecture.MicroService.Messaging.Consumers;
using Wally.Lib.ServiceBus.Abstractions;
using Wally.Lib.ServiceBus.Azure;
using Wally.Lib.ServiceBus.DI.Microsoft;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Extensions;

public static class MessagingExtensions
{
	public static IServiceCollection AddMessaging(this IServiceCollection services, AppSettings settings)
	{
		services.AddPublisher();

		services.Scan(
			a => a.FromAssemblyOf<UserCreatedConsumer>()
				.AddClasses(c => c.AssignableTo(typeof(Consumer<>)))
				.AsImplementedInterfaces()
				.WithScopedLifetime());

		switch (settings.MessageBroker)
		{
			case MessageBrokerType.AzureServiceBus:
				services.AddSingleton(_ => Factory.Create(new Settings(settings.ConnectionStrings.ServiceBus)));
				break;
			case MessageBrokerType.RabbitMQ:
				services.AddSingleton(
					_ => Lib.ServiceBus.RabbitMQ.Factory.Create(
						new Lib.ServiceBus.RabbitMQ.Settings(settings.ConnectionStrings.ServiceBus)));
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(settings.MessageBroker), "Unknown Message Broker");
		}

		services.AddServiceBus();

		return services;
	}

	public static IApplicationBuilder UseMessaging(this IApplicationBuilder app)
	{
		app.UseServiceBus();

		return app;
	}
}
