using Application.DTOs;

namespace Application.Interfaces;

public interface IRealtimeUpdateNotifier
{
    public Task SendDashboardUpdate(ReadingDto reading, CancellationToken cancellationToken = default);
}
