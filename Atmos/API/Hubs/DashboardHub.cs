using Application.DTOs;
using Application.Interfaces;

using Microsoft.AspNetCore.SignalR;

namespace API.Hubs;

public class DashboardHub : Hub
{
    private readonly ILogger<DashboardHub> _logger;

    public DashboardHub(ILogger<DashboardHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
        _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
    }
}
