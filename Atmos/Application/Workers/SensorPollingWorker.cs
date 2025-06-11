using Application.Interfaces;

using AutoMapper;

using Domain.Entities;
using Domain.Interfaces;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Application;

public class SensorPollingWorker : BackgroundService
{
    private readonly ILogger<SensorPollingWorker> _logger;
    private readonly ISensorClient _sensorClient;
    private readonly IAggregator _aggregator;
    private readonly IMapper _mapper;
    private readonly IServiceScopeFactory _scopeFactory;

    public SensorPollingWorker(ILogger<SensorPollingWorker> logger, IAggregator aggregator, ISensorClient sensorClient, IMapper mapper, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _aggregator = aggregator;
        _sensorClient = sensorClient;
        _mapper = mapper;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogDebug("SensorPollingWorker is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var readingRepository = scope.ServiceProvider.GetRequiredService<IReadingRepository>();
                var sensorData = await _sensorClient.GetReadingAsync(stoppingToken);

                _logger.LogDebug("Latest reading received: {reading}", sensorData);

                var reading = _mapper.Map<Reading>(sensorData);
                _ = readingRepository.CreateAsync(reading, stoppingToken);
                await _aggregator.ProcessReadingAsync(reading);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the sensor reading.");
            }

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
}
