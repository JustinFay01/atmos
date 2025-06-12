using Application.DTOs;
using Application.Interfaces;

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
    public Task SendDashboardUpdate(ReadingDto reading, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Sending dashboard update: {reading}", reading);
        return _hubContext.Clients.All.SendAsync("ReceiveDashboardUpdate", reading, cancellationToken);
    }
}
