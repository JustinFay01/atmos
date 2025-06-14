using Application.Interfaces;

using Microsoft.AspNetCore.SignalR;

namespace API.Hubs;

public class DashboardHub : Hub
{
    private readonly ILogger<DashboardHub> _logger;
    private readonly IAggregator _aggregator;

    public DashboardHub(ILogger<DashboardHub> logger, IAggregator aggregator)
    {
        _logger = logger;
        _aggregator = aggregator;
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
        await Clients.Client(Context.ConnectionId).SendAsync("ReceiveDashboardUpdate", _aggregator.LatestUpdate);
        
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
        _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
    }
}
