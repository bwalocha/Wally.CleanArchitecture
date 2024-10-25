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
using Wally.CleanArchitecture.MicroService.Application.Messages.Users;
using Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Models;
using Wally.CleanArchitecture.MicroService.Infrastructure.Messaging;
using Wally.CleanArchitecture.MicroService.Infrastructure.Messaging.Consumers;

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
						services.AddSingleton<IBus, BusStub>();
						break;
					case MessageBrokerType.AzureServiceBus:
						a.UsingAzureServiceBus(
							(host, cfg) =>
							{
								cfg.Host(settings.ConnectionStrings.ServiceBus);
								cfg.ConfigureEndpoints(host, new DefaultEndpointNameFormatter(".", "", true));
							});
						break;
					case MessageBrokerType.Kafka:
						a.UsingInMemory((context, config) => config.ConfigureEndpoints(context));
						a.AddRider(
							rider =>
							{
								rider.AddConsumersFromNamespaceContaining<IInfrastructureMessagingAssemblyMarker>();
								rider.AddProducersFromNamespaceContaining<IApplicationMessagesAssemblyMarker>();

								rider.UsingKafka(
									(context, k) =>
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
						a.UsingRabbitMq(
							(host, cfg) =>
							{
								cfg.Host(new Uri(settings.ConnectionStrings.ServiceBus));
								cfg.ConfigureEndpoints(host, new DefaultEndpointNameFormatter(".", "", true));
							});
						break;
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
		this IKafkaFactoryConfigurator configurator, IRiderRegistrationContext context)
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
		this IKafkaFactoryConfigurator configurator, IRiderRegistrationContext context)
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

		foreach (var @interface in type.GetTypeInfo()
					.ImplementedInterfaces)
		{
			if (@interface.IsGenericType(interfaceType))
			{
				return true;
			}
		}

		return false;
	}

	[SuppressMessage("Major Code Smell", "S4017:Method signatures should not contain nested generic types")]
	private sealed class BusStub : IBus
	{
		private readonly ILogger<BusStub> _logger;

		public BusStub(ILogger<BusStub> logger)
		{
			_logger = logger;
		}

		public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
		{
			throw new NotSupportedException();
		}

		public Task<ISendEndpoint> GetPublishSendEndpoint<T>()
			where T : class
		{
			throw new NotSupportedException();
		}

		public Task Publish<T>(T message, CancellationToken cancellationToken = new())
			where T : class
		{
			_logger.LogWarning("Message Bus is not enabled. The message '{TypeofTName}' has not been sent.",
				typeof(T).Name);

			return Task.CompletedTask;
		}

		public Task Publish<T>(
			T message,
			IPipe<PublishContext<T>> publishPipe,
			CancellationToken cancellationToken = new())
			where T : class
		{
			throw new NotSupportedException();
		}

		public Task Publish<T>(
			T message,
			IPipe<PublishContext> publishPipe,
			CancellationToken cancellationToken = new())
			where T : class
		{
			throw new NotSupportedException();
		}

		public Task Publish(object message, CancellationToken cancellationToken = new())
		{
			throw new NotSupportedException();
		}

		public Task Publish(
			object message,
			IPipe<PublishContext> publishPipe,
			CancellationToken cancellationToken = new())
		{
			throw new NotSupportedException();
		}

		public Task Publish(object message, Type messageType, CancellationToken cancellationToken = new())
		{
			throw new NotSupportedException();
		}

		public Task Publish(
			object message,
			Type messageType,
			IPipe<PublishContext> publishPipe,
			CancellationToken cancellationToken = new())
		{
			throw new NotSupportedException();
		}

		public Task Publish<T>(object values, CancellationToken cancellationToken = new())
			where T : class
		{
			throw new NotSupportedException();
		}

		public Task Publish<T>(
			object values,
			IPipe<PublishContext<T>> publishPipe,
			CancellationToken cancellationToken = new())
			where T : class
		{
			throw new NotSupportedException();
		}

		public Task Publish<T>(
			object values,
			IPipe<PublishContext> publishPipe,
			CancellationToken cancellationToken = new())
			where T : class
		{
			throw new NotSupportedException();
		}

		public ConnectHandle ConnectSendObserver(ISendObserver observer)
		{
			throw new NotSupportedException();
		}

		public Task<ISendEndpoint> GetSendEndpoint(Uri address)
		{
			throw new NotSupportedException();
		}

		public ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
			where T : class
		{
			throw new NotSupportedException();
		}

		public ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe, ConnectPipeOptions options)
			where T : class
		{
			throw new NotSupportedException();
		}

		public ConnectHandle ConnectRequestPipe<T>(Guid requestId, IPipe<ConsumeContext<T>> pipe)
			where T : class
		{
			throw new NotSupportedException();
		}

		public ConnectHandle ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer)
			where T : class
		{
			throw new NotSupportedException();
		}

		public ConnectHandle ConnectConsumeObserver(IConsumeObserver observer)
		{
			throw new NotSupportedException();
		}

		public ConnectHandle ConnectReceiveObserver(IReceiveObserver observer)
		{
			throw new NotSupportedException();
		}

		public ConnectHandle ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
		{
			throw new NotSupportedException();
		}

		public ConnectHandle ConnectEndpointConfigurationObserver(IEndpointConfigurationObserver observer)
		{
			throw new NotSupportedException();
		}

		public HostReceiveEndpointHandle ConnectReceiveEndpoint(
			IEndpointDefinition definition,
			IEndpointNameFormatter? endpointNameFormatter = null,
			Action<IReceiveEndpointConfigurator>? configureEndpoint = null)
		{
			throw new NotSupportedException();
		}

		public HostReceiveEndpointHandle ConnectReceiveEndpoint(
			string queueName,
			Action<IReceiveEndpointConfigurator>? configureEndpoint = null)
		{
			throw new NotSupportedException();
		}

		public void Probe(ProbeContext context)
		{
			throw new NotSupportedException();
		}

		public Uri Address { get; } = null!;

		public IBusTopology Topology { get; } = null!;
	}

	[SuppressMessage("Major Code Smell", "S4017:Method signatures should not contain nested generic types")]
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

		public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
		{
			throw new NotSupportedException();
		}

		public Task<ISendEndpoint> GetPublishSendEndpoint<T>()
			where T : class
		{
			throw new NotSupportedException();
		}

		public async Task Publish<T>(T message, CancellationToken cancellationToken = new())
			where T : class
		{
			_logger.LogDebug("Publishing {FullName}", typeof(T).FullName);

			var topicProducer = _serviceProvider.GetRequiredService<ITopicProducer<T>>();
			await topicProducer.Produce(message, cancellationToken);
		}

		public Task Publish<T>(
			T message,
			IPipe<PublishContext<T>> publishPipe,
			CancellationToken cancellationToken = new())
			where T : class
		{
			throw new NotSupportedException();
		}

		public Task Publish<T>(
			T message,
			IPipe<PublishContext> publishPipe,
			CancellationToken cancellationToken = new())
			where T : class
		{
			throw new NotSupportedException();
		}

		public Task Publish(object message, CancellationToken cancellationToken = new())
		{
			throw new NotSupportedException();
		}

		public Task Publish(
			object message,
			IPipe<PublishContext> publishPipe,
			CancellationToken cancellationToken = new())
		{
			throw new NotSupportedException();
		}

		public Task Publish(object message, Type messageType, CancellationToken cancellationToken = new())
		{
			throw new NotSupportedException();
		}

		public Task Publish(
			object message,
			Type messageType,
			IPipe<PublishContext> publishPipe,
			CancellationToken cancellationToken = new())
		{
			throw new NotSupportedException();
		}

		public Task Publish<T>(object values, CancellationToken cancellationToken = new())
			where T : class
		{
			throw new NotSupportedException();
		}

		public Task Publish<T>(
			object values,
			IPipe<PublishContext<T>> publishPipe,
			CancellationToken cancellationToken = new())
			where T : class
		{
			throw new NotSupportedException();
		}

		public Task Publish<T>(
			object values,
			IPipe<PublishContext> publishPipe,
			CancellationToken cancellationToken = new())
			where T : class
		{
			throw new NotSupportedException();
		}

		public ConnectHandle ConnectSendObserver(ISendObserver observer)
		{
			throw new NotSupportedException();
		}

		public Task<ISendEndpoint> GetSendEndpoint(Uri address)
		{
			throw new NotSupportedException();
		}

		public ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
			where T : class
		{
			throw new NotSupportedException();
		}

		public ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe, ConnectPipeOptions options)
			where T : class
		{
			throw new NotSupportedException();
		}

		public ConnectHandle ConnectRequestPipe<T>(Guid requestId, IPipe<ConsumeContext<T>> pipe)
			where T : class
		{
			throw new NotSupportedException();
		}

		public ConnectHandle ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer)
			where T : class
		{
			throw new NotSupportedException();
		}

		public ConnectHandle ConnectConsumeObserver(IConsumeObserver observer)
		{
			throw new NotSupportedException();
		}

		public ConnectHandle ConnectReceiveObserver(IReceiveObserver observer)
		{
			throw new NotSupportedException();
		}

		public ConnectHandle ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
		{
			throw new NotSupportedException();
		}

		public ConnectHandle ConnectEndpointConfigurationObserver(IEndpointConfigurationObserver observer)
		{
			throw new NotSupportedException();
		}

		public HostReceiveEndpointHandle ConnectReceiveEndpoint(
			IEndpointDefinition definition,
			IEndpointNameFormatter? endpointNameFormatter = null,
			Action<IReceiveEndpointConfigurator>? configureEndpoint = null)
		{
			throw new NotSupportedException();
		}

		public HostReceiveEndpointHandle ConnectReceiveEndpoint(
			string queueName,
			Action<IReceiveEndpointConfigurator>? configureEndpoint = null)
		{
			throw new NotSupportedException();
		}

		public void Probe(ProbeContext context)
		{
			throw new NotSupportedException();
		}

		public Uri Address { get; } = null!;

		public IBusTopology Topology { get; } = null!;
	}
}
