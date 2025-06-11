using Application.DTOs;
using Application.Interfaces;

using Microsoft.AspNetCore.SignalR;

namespace API.Hubs;

public class DashboardHub : Hub, IRealtimeUpdateNotifier
{
    public async Task SendDashboardUpdate(ReadingDto reading)
    {
        await Clients.All.SendAsync("ReceiveDashboardUpdate", reading);
    }
}
