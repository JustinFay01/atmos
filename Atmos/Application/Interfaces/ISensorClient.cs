using Application.DTOs;

namespace Application.Interfaces;

public interface ISensorClient
{
    public bool IsConnected { get; }
    public Task ConnectAsync(CancellationToken cancellationToken);
    public Task DisconnectAsync(CancellationToken cancellationToken);
    public Task<ReadingDto> GetReadingAsync(CancellationToken cancellationToken);

}
