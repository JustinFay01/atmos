using Application.Dtos;

namespace Application.Interfaces;

public interface IRealtimeUpdateNotifier
{
    public Task SendDashboardUpdateAsync(ReadingAggregateDto update, CancellationToken cancellationToken = default);
}
