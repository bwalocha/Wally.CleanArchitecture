using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper.Internal;
using Confluent.Kafka;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Wally.CleanArchitecture.MicroService.Application.Messages;
using Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Models;
using Wally.CleanArchitecture.MicroService.Infrastructure.Messaging;
using Wally.CleanArchitecture.MicroService.Infrastructure.Persistence;
using IBus = Wally.CleanArchitecture.MicroService.Application.Abstractions.IBus;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Extensions;

public static class MessagingExtensions
{
	public static IServiceCollection AddMessaging(this IServiceCollection services, AppSettings settings)
	{
		services.AddMassTransit(a =>
		{
			a.AddConsumers(typeof(IInfrastructureMessagingAssemblyMarker).Assembly);

			switch (settings.MessageBroker)
			{
				case MessageBrokerType.None:
					services.AddSingleton<IBus, BusStub>();
					break;
				case MessageBrokerType.AzureServiceBus:
					a.UsingAzureServiceBus((host, cfg) =>
					{
						cfg.Host(settings.ConnectionStrings.ServiceBus);
						cfg.ConfigureEndpoints(host, new DefaultEndpointNameFormatter(".", "", true));
					});
					break;
				case MessageBrokerType.Kafka:
					a.UsingInMemory((context, config) => config.ConfigureEndpoints(context));
					a.AddEntityFrameworkOutbox<ApplicationDbContext>(o => o.UseBusOutbox());
					a.AddConfigureEndpointsCallback((context, _, cfg) =>
					{
						cfg.UseEntityFrameworkOutbox<ApplicationDbContext>(context);
					});
					a.AddRider(rider =>
					{
						// Kafka is not intended to create topology during startup.
						// Topics should be created with correct number of partitions and replicas beforehand
						// https://masstransit.io/documentation/configuration/transports/kafka#configure-topology
						rider.AddProducersFromNamespaceContaining<IApplicationMessagesAssemblyMarker>();
						rider.AddConsumersFromNamespaceContaining<IInfrastructureMessagingAssemblyMarker>();

						rider.UsingKafka((context, k) =>
						{
							k.ClientId = typeof(IInfrastructureMessagingAssemblyMarker).Namespace;
							k.Host(settings.ConnectionStrings.ServiceBus);

							k.AddConsumersFromNamespaceContaining<IInfrastructureMessagingAssemblyMarker>(
								context);
						});

						services.AddScoped<IBus, KafkaBus>();
					});
					break;
				case MessageBrokerType.RabbitMQ:
					a.UsingRabbitMq((host, cfg) =>
					{
						cfg.Host(new Uri(settings.ConnectionStrings.ServiceBus));
						cfg.ConfigureEndpoints(host, new DefaultEndpointNameFormatter(".", "", true));
					});
					break;
				case MessageBrokerType.Unknown:
				default:
					throw new NotSupportedException(
						$"Not supported Message Broker type: '{settings.MessageBroker}'");
			}
		});

		return services;
	}

	private static IRiderRegistrationConfigurator AddProducersFromNamespaceContaining<TMessagesAssembly>(
		this IRiderRegistrationConfigurator configurator)
	{
		foreach (var type in typeof(TMessagesAssembly).Assembly.GetTypes()
					.Where(a => a.Name.EndsWith("Message")))
		{
			var methodInfo = typeof(KafkaProducerRegistrationExtensions)
				.GetMethods()
				.Where(a => a
					.Name == nameof(KafkaProducerRegistrationExtensions.AddProducer))
				.Where(a => a.GetGenericArguments()
					.Length == 1)
				.Single(a => a.GetParameters()
					.Length == 3);
			var genericMethodInfo = methodInfo.MakeGenericMethod(type);

			genericMethodInfo.Invoke(null, [configurator, type.FullName, null,]);
		}

		return configurator;
	}

	private static IKafkaFactoryConfigurator AddConsumersFromNamespaceContaining<TConsumersAssembly>(
		this IKafkaFactoryConfigurator configurator, IRegistrationContext context)
	{
		foreach (var type in typeof(TConsumersAssembly).Assembly.GetTypes()
					.Where(a => a.ImplementsGenericInterface(typeof(IConsumer<>))))
		{
			var messageType = type.GetInterfaces()
				.Single(a => a.IsGenericType)
				.GenericTypeArguments.Single();
			if (messageType.IsGenericType)
			{
				continue;
			}

			var methodInfo =
				typeof(MessagingExtensions).GetMethod(nameof(AddConsumer),
					BindingFlags.Static | BindingFlags.NonPublic) !;
			var genericMethodInfo = methodInfo.MakeGenericMethod(messageType, type);

			genericMethodInfo.Invoke(null, [configurator, context,]);
		}

		return configurator;
	}

	private static IKafkaFactoryConfigurator AddConsumer<TMessage, TConsumer>(
		this IKafkaFactoryConfigurator configurator, IRegistrationContext context)
		where TMessage : class
		where TConsumer : class, IConsumer
	{
		configurator.TopicEndpoint<TMessage>(typeof(TMessage).FullName,
			typeof(IInfrastructureMessagingAssemblyMarker).Namespace,
			a =>
			{
				a.AutoOffsetReset = AutoOffsetReset.Earliest;
				a.ConfigureConsumer<TConsumer>(context);
			});

		return configurator;
	}

	private static bool ImplementsGenericInterface(this Type type, Type interfaceType)
	{
		if (!interfaceType.IsInterface)
		{
			throw new ArgumentException($"Parameter '{nameof(interfaceType)}' is not an Interface");
		}

		if (type.IsGenericType(interfaceType))
		{
			return true;
		}

		return type.GetTypeInfo()
			.ImplementedInterfaces.Any(@interface => @interface.IsGenericType(interfaceType));
	}

	[SuppressMessage("Major Code Smell", "S1144:Unused private types or members should be removed")]
	private sealed class BusStub : IBus
	{
		private readonly ILogger<BusStub> _logger;

		public BusStub(ILogger<BusStub> logger)
		{
			_logger = logger;
		}

		public Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken)
			where TMessage : class
		{
			_logger.LogWarning("Message Bus is not enabled. The message '{TypeofTName}' has not been sent",
				typeof(TMessage).Name);

			return Task.CompletedTask;
		}
	}

	[SuppressMessage("Major Code Smell", "S1144:Unused private types or members should be removed")]
	private sealed class KafkaBus : IBus
	{
		private readonly ILogger<KafkaBus> _logger;
		private readonly IServiceProvider _serviceProvider;

		public KafkaBus(IServiceProvider serviceProvider, ILogger<KafkaBus> logger)
		{
			_serviceProvider = serviceProvider;
			_logger = logger;
		}

		public async Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken)
			where TMessage : class
		{
			_logger.LogDebug("Publishing {FullName}", typeof(TMessage).FullName);

			var topicProducer = _serviceProvider.GetRequiredService<ITopicProducer<TMessage>>();
			await topicProducer.Produce(message, cancellationToken);
		}
	}
}
