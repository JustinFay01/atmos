using Application.Interfaces;
using Application.Services;

using Microsoft.AspNetCore.SignalR;

namespace API.Hubs;

public class DashboardHub(
    ILogger<DashboardHub> logger,
    IAggregator aggregator,
    IHourlyReadingService hourlyReadingService)
    : Hub
{
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
        
        await Task.WhenAll([
            Clients.Client(Context.ConnectionId)
                .SendAsync(HubSubscription.ReceiveDashboardUpdate, aggregator.AggregatedReading),
            Clients.Client(Context.ConnectionId)
                .SendAsync(HubSubscription.ReceiveHourlyUpdate, hourlyReadingService.HourlyReadings)
        ]);

    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
        logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
    }
}
