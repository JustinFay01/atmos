namespace Application.Interfaces;

public interface IRealtimeUpdateNotifier
{
    public Task SendDashboardUpdateAsync(object update, CancellationToken cancellationToken = default);
}
