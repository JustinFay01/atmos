using Application.DTOs;

using Microsoft.AspNetCore.SignalR;

namespace API.Hubs;

public class DashboardHub : Hub
{
    public async Task SendDashboardUpdate(ReadingDto reading)
    {
        await Clients.All.SendAsync("ReceiveDashboardUpdate", reading);
    }
}
