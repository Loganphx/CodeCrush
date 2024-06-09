using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR;

[Authorize]
public class PresenceHub : Hub
{
    private readonly PresenceTracker _presenceTracker;

    public PresenceHub(PresenceTracker presenceTracker)
    {
        _presenceTracker = presenceTracker;
    }
    
    public override async Task OnConnectedAsync()
    {
        var username = Context.User.GetUsername();
        await _presenceTracker.UserConnected(username, Context.ConnectionId);
        await Clients.Others.SendAsync("UserIsOnline", username);

        var currentUsers = await _presenceTracker.GetOnlineUsers();
        await Clients.All.SendAsync("GetOnlineUsers", currentUsers);
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var username = Context.User.GetUsername();
        
        await _presenceTracker.UserDisconnected(username, Context.ConnectionId);
        await Clients.Others.SendAsync("UserIsOffline", username);

        var currentUsers = await _presenceTracker.GetOnlineUsers();
        await Clients.All.SendAsync("GetOnlineUsers", currentUsers);

        await base.OnDisconnectedAsync(exception);
    }
}