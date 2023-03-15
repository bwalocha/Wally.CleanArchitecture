using System;

using MassTransit;

using Microsoft.Extensions.DependencyInjection;

using Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Models;
using Wally.CleanArchitecture.MicroService.Infrastructure.Messaging;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Extensions;

public static class MessagingExtensions
{
	public static IServiceCollection AddMessaging(this IServiceCollection services, AppSettings settings)
	{
		services.AddMassTransit(
			a =>
			{
				a.AddConsumers(typeof(IInfrastructureMessagingAssemblyMarker).Assembly);

				switch (settings.MessageBroker)
				{
					case MessageBrokerType.None:
						break; // TODO: register Stub
					case MessageBrokerType.AzureServiceBus:
						a.UsingAzureServiceBus(
							(host, cfg) =>
							{
								cfg.Host(settings.ConnectionStrings.ServiceBus);
								cfg.ConfigureEndpoints(host, new DefaultEndpointNameFormatter(".", "", true));
							});
						break;
					case MessageBrokerType.RabbitMQ:
						a.UsingRabbitMq(
							(host, cfg) =>
							{
								cfg.Host(new Uri(settings.ConnectionStrings.ServiceBus));
								cfg.ConfigureEndpoints(host, new DefaultEndpointNameFormatter(".", "", true));
							});
						break;
					default:
						throw new ArgumentOutOfRangeException(nameof(settings.MessageBroker), "Unknown Message Broker");
				}
			});

		return services;
	}
}
