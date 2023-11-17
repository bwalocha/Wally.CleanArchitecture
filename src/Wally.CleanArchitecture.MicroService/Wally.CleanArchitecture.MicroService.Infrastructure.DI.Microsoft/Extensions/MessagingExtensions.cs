using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

using MassTransit;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
								// TODO: auto-register
								rider.AddProducer<UserCreatedMessage>(nameof(UserCreatedMessage));
								// rider.AddProducer<UserUpdatedMessage>(nameof(FileModifiedMessage));

								rider.UsingKafka(
									(context, k) =>
									{
										k.ClientId = typeof(IInfrastructureMessagingAssemblyMarker).Namespace;
										k.Host(settings.ConnectionStrings.ServiceBus);

										// TODO: auto-register
										k.TopicEndpoint<UserCreatedMessage>(typeof(IInfrastructureMessagingAssemblyMarker).Namespace, typeof(IInfrastructureMessagingAssemblyMarker).Namespace, e =>
										{
											e.ConfigureConsumer<UserCreatedMessageConsumer>(context);
										});
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
						throw new ArgumentOutOfRangeException(
							nameof(settings.MessageBroker),
							$"Unknown Message Broker: '{settings.MessageBroker}'");
				}
			});

		return services;
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

		public Task<ISendEndpoint> GetPublishSendEndpoint<T>() where T : class
		{
			throw new NotSupportedException();
		}

		public Task Publish<T>(T message, CancellationToken cancellationToken = new()) where T : class
		{
			_logger.LogWarning($"Message Bus is not enabled. The message '{typeof(T)}' has not been sent.");

			return Task.CompletedTask;
		}

		public Task Publish<T>(
			T message,
			IPipe<PublishContext<T>> publishPipe,
			CancellationToken cancellationToken = new()) where T : class
		{
			throw new NotSupportedException();
		}

		public Task Publish<T>(
			T message,
			IPipe<PublishContext> publishPipe,
			CancellationToken cancellationToken = new()) where T : class
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

		public Task Publish<T>(object values, CancellationToken cancellationToken = new()) where T : class
		{
			throw new NotSupportedException();
		}

		public Task Publish<T>(
			object values,
			IPipe<PublishContext<T>> publishPipe,
			CancellationToken cancellationToken = new()) where T : class
		{
			throw new NotSupportedException();
		}

		public Task Publish<T>(
			object values,
			IPipe<PublishContext> publishPipe,
			CancellationToken cancellationToken = new()) where T : class
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

		public ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe) where T : class
		{
			throw new NotSupportedException();
		}

		public ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe, ConnectPipeOptions options)
			where T : class
		{
			throw new NotSupportedException();
		}

		public ConnectHandle ConnectRequestPipe<T>(Guid requestId, IPipe<ConsumeContext<T>> pipe) where T : class
		{
			throw new NotSupportedException();
		}

		public ConnectHandle ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer) where T : class
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

		public Task<ISendEndpoint> GetPublishSendEndpoint<T>() where T : class
		{
			throw new NotSupportedException();
		}

		public async Task Publish<T>(T message, CancellationToken cancellationToken = new()) where T : class
		{
			_logger.LogDebug("Publishing {FullName}", typeof(T).FullName);

			var topicProducer = _serviceProvider.GetRequiredService<ITopicProducer<T>>();
			await topicProducer.Produce(message, cancellationToken);
		}

		public Task Publish<T>(
			T message,
			IPipe<PublishContext<T>> publishPipe,
			CancellationToken cancellationToken = new()) where T : class
		{
			throw new NotSupportedException();
		}

		public Task Publish<T>(
			T message,
			IPipe<PublishContext> publishPipe,
			CancellationToken cancellationToken = new()) where T : class
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

		public Task Publish<T>(object values, CancellationToken cancellationToken = new()) where T : class
		{
			throw new NotSupportedException();
		}

		public Task Publish<T>(
			object values,
			IPipe<PublishContext<T>> publishPipe,
			CancellationToken cancellationToken = new()) where T : class
		{
			throw new NotSupportedException();
		}

		public Task Publish<T>(
			object values,
			IPipe<PublishContext> publishPipe,
			CancellationToken cancellationToken = new()) where T : class
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

		public ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe) where T : class
		{
			throw new NotSupportedException();
		}

		public ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe, ConnectPipeOptions options)
			where T : class
		{
			throw new NotSupportedException();
		}

		public ConnectHandle ConnectRequestPipe<T>(Guid requestId, IPipe<ConsumeContext<T>> pipe) where T : class
		{
			throw new NotSupportedException();
		}

		public ConnectHandle ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer) where T : class
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
