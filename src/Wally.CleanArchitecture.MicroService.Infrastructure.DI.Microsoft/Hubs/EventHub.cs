using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Hubs;

public class EventHub : Hub<IEventHub>
{
	public override async Task OnConnectedAsync()
	{
		await Groups.AddToGroupAsync(Context.ConnectionId, "SignalR Users");
		await Clients.Caller.NewEventAsync(Context.ConnectionId, "Hi!", CancellationToken.None);
		await Clients.Others.NewEventAsync(Context.ConnectionId, "Member has joined.", CancellationToken.None);
		await base.OnConnectedAsync();
	}

	public override async Task OnDisconnectedAsync(Exception? exception)
	{
		await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SignalR Users");
		await Clients.Others.NewEventAsync(Context.ConnectionId, "Member has disconnected.", CancellationToken.None);
		await base.OnDisconnectedAsync(exception);
	}

	// TODO: to remove?
	/*public Task ThrowException()
	{
		throw new HubException("This error will be sent to the client!");
	}*/

	public async Task SendEventAsync(string user, string @event, CancellationToken cancellationToken = default)
	{
		await Clients.All.NewEventAsync(user, @event, cancellationToken);
	}
}
