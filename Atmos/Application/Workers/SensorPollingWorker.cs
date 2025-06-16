using Application.Extensions;
using Application.Helper;
using Application.Interfaces;

using AutoMapper;

using Domain.Entities;
using Domain.Interfaces;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Application.Workers;

public class SensorPollingWorker : BackgroundService
{
    private readonly ILogger<SensorPollingWorker> _logger;
    private readonly ISensorClient _sensorClient;
    private readonly IAggregator _aggregator;
    private readonly IMapper _mapper;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IRealtimeUpdateNotifier _notifier;

    private Task? _orchestrationTask;
    private readonly TimeSpan _pollingInterval = TimeSpan.FromSeconds(10);
    private readonly TimeSpan _processingTimeout = TimeSpan.FromSeconds(9);
    // Delay by 150 (:00.150) to ensure we don't drift backwards into :59.99
    private const int DELAY_CUSHION_MS = 150;

    public SensorPollingWorker(ILogger<SensorPollingWorker> logger, IAggregator aggregator, ISensorClient sensorClient, IMapper mapper, IServiceScopeFactory scopeFactory, IRealtimeUpdateNotifier notifier)
    {
        _logger = logger;
        _aggregator = aggregator;
        _sensorClient = sensorClient;
        _mapper = mapper;
        _scopeFactory = scopeFactory;
        _notifier = notifier;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("SensorPollingWorker has been requested to start. Waiting until the nearest 10 second mark.");

        var delay = DateTimeProvider.Instance.MillisecondsTillTenSeconds();
        if (delay == 0)
        {
            _logger.LogDebug("No delay needed, starting immediately.");
        }
        else
        {
            _logger.LogDebug("Delaying start by {Delay} milliseconds.", delay);
            await Task.Delay((int)delay + DELAY_CUSHION_MS, cancellationToken);
        }
        await base.StartAsync(cancellationToken);
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogDebug("SensorPollingWorker is starting.");

        using var timer = new PeriodicTimer(_pollingInterval);

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                _logger.LogInformation("Worker tick at: {Time}", DateTimeProvider.Instance.Now);

                if (_orchestrationTask is { IsCompleted: false })
                {
                    _logger.LogWarning("Previous sensor data processing is still in progress. Skipping this tick.");
                    continue;
                }

                _orchestrationTask = OrchestrateSensorDataAsync(stoppingToken);
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("SensorPollingWorker has been cancelled.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred in SensorPollingWorker.");
        }
        finally
        {
            _logger.LogDebug("SensorPollingWorker is stopping.");
        }
    }

    private async Task OrchestrateSensorDataAsync(CancellationToken stoppingToken)
    {
        _logger.LogDebug("Orchestrating sensor data processing.");

        using var workCts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);

        workCts.CancelAfter(_processingTimeout);

        try
        {
            workCts.Token.ThrowIfCancellationRequested();

            using var scope = _scopeFactory.CreateScope();
            var readingRepository = scope.ServiceProvider.GetRequiredService<IReadingAggregateRepository>();

            // Get Raw Sensor Data
            _logger.LogDebug("Fetching latest sensor reading.");
            var sensorData = await _sensorClient.GetReadingAsync(workCts.Token);
            _logger.LogDebug("Latest reading received: {reading}", sensorData);

            // Process and Aggregate Data
            var aggregatedReadingDto = await _aggregator.AggregateRawReading(sensorData, workCts.Token);
            var aggregate = _mapper.Map<ReadingAggregate>(aggregatedReadingDto);

            // Store Aggregated Data
            await readingRepository.CreateAsync(aggregate, workCts.Token);

            // Notify Clients of Update
            await _notifier.SendDashboardUpdateAsync(aggregatedReadingDto, workCts.Token);

            _logger.LogInformation("Sensor data processed successfully.");
        }
        catch (OperationCanceledException operationCanceledException)
        {
            if (stoppingToken.IsCancellationRequested && operationCanceledException.CancellationToken == workCts.Token)
            {
                _logger.LogInformation("Sensor data processing was cancelled by the stopping token.");
            }
            else if (operationCanceledException.CancellationToken == workCts.Token)
            {
                _logger.LogWarning("Sensor data processing timed out after {Timeout} and was cancelled. Attempting to cancel and skip this iteration.", _processingTimeout);
            }
            else
            {
                _logger.LogWarning(operationCanceledException, "Sensor data processing was cancelled due to an unexpected cancellation request.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing the sensor reading.");
        }
        finally
        {
            _logger.LogDebug("Finished attempt to process sensor data.");
        }
    }
}
