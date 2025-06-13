using Application.Models;

namespace Application.Interfaces;

public interface IRealtimeUpdateNotifier
{
    public Task SendDashboardUpdateAsync(DashboardUpdate update, CancellationToken cancellationToken = default);
}
