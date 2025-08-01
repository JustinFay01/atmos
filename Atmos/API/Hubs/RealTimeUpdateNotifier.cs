using Application.Dtos;
using Application.Interfaces;
using Application.Models;

using Microsoft.AspNetCore.SignalR;

namespace API.Hubs;

public class RealTimeUpdateNotifier : IRealtimeUpdateNotifier
{
    private readonly ILogger<RealTimeUpdateNotifier> _logger;
    private readonly IHubContext<DashboardHub> _hubContext;

    public RealTimeUpdateNotifier(ILogger<RealTimeUpdateNotifier> logger, IHubContext<DashboardHub> hubContext)
    {
        _logger = logger;
        _hubContext = hubContext;
    }
    public Task SendDashboardUpdateAsync(ReadingAggregateDto update, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Sending dashboard update: {update}", update);
        return _hubContext.Clients.All.SendAsync(HubSubscription.ReceiveDashboardUpdate, update, cancellationToken);
    }
    
    public Task SendHourUpdateAsync(IList<HourReadingDto?> update, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Sending hourly update: {update}", update);
        return _hubContext.Clients.All.SendAsync(HubSubscription.ReceiveHourlyUpdate, update, cancellationToken);
    }
}
