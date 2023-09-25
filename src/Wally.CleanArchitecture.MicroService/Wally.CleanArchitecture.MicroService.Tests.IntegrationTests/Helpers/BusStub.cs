using System;
using System.Threading;
using System.Threading.Tasks;

using MassTransit;

namespace Wally.CleanArchitecture.MicroService.Tests.IntegrationTests.Helpers;

public class BusStub : IBus
{
	public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
	{
		throw new NotImplementedException();
	}

	public Task<ISendEndpoint> GetPublishSendEndpoint<T>() where T : class
	{
		throw new NotImplementedException();
	}

	public Task Publish<T>(T message, CancellationToken cancellationToken = new()) where T : class
	{
		return Task.CompletedTask;
	}

	public Task Publish<T>(T message, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken = new())
		where T : class
	{
		throw new NotImplementedException();
	}

	public Task Publish<T>(T message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = new())
		where T : class
	{
		throw new NotImplementedException();
	}

	public Task Publish(object message, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public Task Publish(object message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public Task Publish(object message, Type messageType, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public Task Publish(
		object message,
		Type messageType,
		IPipe<PublishContext> publishPipe,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public Task Publish<T>(object values, CancellationToken cancellationToken = new()) where T : class
	{
		throw new NotImplementedException();
	}

	public Task Publish<T>(
		object values,
		IPipe<PublishContext<T>> publishPipe,
		CancellationToken cancellationToken = new()) where T : class
	{
		throw new NotImplementedException();
	}

	public Task Publish<T>(
		object values,
		IPipe<PublishContext> publishPipe,
		CancellationToken cancellationToken = new()) where T : class
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

	public ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe, ConnectPipeOptions options)
		where T : class
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

	public HostReceiveEndpointHandle ConnectReceiveEndpoint(
		string queueName,
		Action<IReceiveEndpointConfigurator>? configureEndpoint = null)
	{
		throw new NotImplementedException();
	}

	public void Probe(ProbeContext context)
	{
		throw new NotImplementedException();
	}

	public Uri Address { get; } = new("http://localhost");

	public IBusTopology Topology { get; } = null!;
}
