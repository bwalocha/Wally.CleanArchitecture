using System;
using System.Threading;
using System.Threading.Tasks;

using MassTransit;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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

	private class BusStub : IBus
	{
		private readonly ILogger<BusStub> _logger;

		public BusStub(ILogger<BusStub> logger)
		{
			_logger = logger;
		}
		
		public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
		{
			throw new NotImplementedException();
		}

		public Task<ISendEndpoint> GetPublishSendEndpoint<T>() where T : class
		{
			throw new NotImplementedException();
		}

		public Task Publish<T>(T message, CancellationToken cancellationToken = new CancellationToken()) where T : class
		{
			_logger.LogWarning($"Message Bus is not enabled. The message '{typeof(T)}' has not been sent.");
			
			return Task.CompletedTask;
		}

		public Task Publish<T>(T message, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken = new CancellationToken()) where T : class
		{
			throw new NotImplementedException();
		}

		public Task Publish<T>(T message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = new CancellationToken()) where T : class
		{
			throw new NotImplementedException();
		}

		public Task Publish(object message, CancellationToken cancellationToken = new CancellationToken())
		{
			throw new NotImplementedException();
		}

		public Task Publish(object message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = new CancellationToken())
		{
			throw new NotImplementedException();
		}

		public Task Publish(object message, Type messageType, CancellationToken cancellationToken = new CancellationToken())
		{
			throw new NotImplementedException();
		}

		public Task Publish(
			object message,
			Type messageType,
			IPipe<PublishContext> publishPipe,
			CancellationToken cancellationToken = new CancellationToken())
		{
			throw new NotImplementedException();
		}

		public Task Publish<T>(object values, CancellationToken cancellationToken = new CancellationToken()) where T : class
		{
			throw new NotImplementedException();
		}

		public Task Publish<T>(object values, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken = new CancellationToken()) where T : class
		{
			throw new NotImplementedException();
		}

		public Task Publish<T>(object values, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = new CancellationToken()) where T : class
		{
			throw new NotImplementedException();
		}

		public ConnectHandle ConnectSendObserver(ISendObserver observer)
		{
			throw new NotImplementedException();
		}

		public Task<ISendEndpoint> GetSendEndpoint(Uri address)
		{
			throw new NotImplementedException();
		}

		public ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe) where T : class
		{
			throw new NotImplementedException();
		}

		public ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe, ConnectPipeOptions options) where T : class
		{
			throw new NotImplementedException();
		}

		public ConnectHandle ConnectRequestPipe<T>(Guid requestId, IPipe<ConsumeContext<T>> pipe) where T : class
		{
			throw new NotImplementedException();
		}

		public ConnectHandle ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer) where T : class
		{
			throw new NotImplementedException();
		}

		public ConnectHandle ConnectConsumeObserver(IConsumeObserver observer)
		{
			throw new NotImplementedException();
		}

		public ConnectHandle ConnectReceiveObserver(IReceiveObserver observer)
		{
			throw new NotImplementedException();
		}

		public ConnectHandle ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
		{
			throw new NotImplementedException();
		}

		public ConnectHandle ConnectEndpointConfigurationObserver(IEndpointConfigurationObserver observer)
		{
			throw new NotImplementedException();
		}

		public HostReceiveEndpointHandle ConnectReceiveEndpoint(
			IEndpointDefinition definition,
			IEndpointNameFormatter? endpointNameFormatter = null,
			Action<IReceiveEndpointConfigurator>? configureEndpoint = null)
		{
			throw new NotImplementedException();
		}

		public HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator>? configureEndpoint = null)
		{
			throw new NotImplementedException();
		}

		public void Probe(ProbeContext context)
		{
			throw new NotImplementedException();
		}

		public Uri Address { get; }

		public IBusTopology Topology { get; }
	}
}
