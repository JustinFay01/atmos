using Application.Helper;
using Application.Interfaces;
using Application.Models;
using Application.Services;

using AutoMapper;

using Domain.Entities;
using Domain.Interfaces;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Application.Workers;

public class SensorPollingWorker(
    ILogger<SensorPollingWorker> logger,
    SensorSettings sensorSettings,
    IAggregator aggregator,
    ISensorClient sensorClient,
    IMapper mapper,
    IServiceScopeFactory scopeFactory,
    IHourlyReadingService hourlyReadingService,
    IRealtimeUpdateNotifier notifier)
    : BackgroundService
{
    private Task? _orchestrationTask;
    private readonly TimeSpan _processingTimeout = TimeSpan.FromSeconds(9);
    private const int MaxRetryDelayMs = 2000; // Maximum delay between retries in milliseconds

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogDebug("SensorPollingWorker is starting.");
        await AttemptToConnectSensorClientAsync(cancellationToken);

        if (!sensorClient.IsConnected)
        {
            await StopAsync(cancellationToken);
            return;
        }

        if (sensorSettings.WaitTillNearestTenSeconds)
        {
            await DelayUntilNextTickAsync(cancellationToken);
        }

        await base.StartAsync(cancellationToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync(cancellationToken);
        logger.LogDebug("SensorPollingWorker has stopped and will not process any more sensor data.");
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogDebug("SensorPollingWorker is starting.");
        
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Worker tick at: {Time}", DateTimeProvider.Instance.Now);

                switch (_orchestrationTask)
                {
                    case { IsCompleted: false }:
                        logger.LogWarning("Previous sensor data processing is still in progress. Skipping this tick.");
                        continue;
                    case { IsFaulted: true }:
                        throw _orchestrationTask.Exception;
                    default:
                        _orchestrationTask = OrchestrateSensorDataAsync(stoppingToken);
                        break;
                }

                await DelayUntilNextTickAsync(stoppingToken);
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("SensorPollingWorker has been cancelled.");
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "An error occurred in SensorPollingWorker. The application is shutting down.");
            await StopAsync(stoppingToken);
        }
        finally
        {
            logger.LogDebug("SensorPollingWorker is stopping.");
        }
    }

    private async Task OrchestrateSensorDataAsync(CancellationToken stoppingToken)
    {
        logger.LogDebug("Orchestrating sensor data processing.");

        using var workCts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);

        workCts.CancelAfter(_processingTimeout);

        try
        {
            workCts.Token.ThrowIfCancellationRequested();

            using var scope = scopeFactory.CreateScope();
            var readingRepository = scope.ServiceProvider.GetRequiredService<IReadingAggregateRepository>();

            // Get Raw Sensor Data
            logger.LogDebug("Fetching latest sensor reading.");
            var sensorData = await sensorClient.GetReadingAsync(workCts.Token);
            logger.LogDebug("Latest reading received: {reading}", sensorData);

            // Process and Aggregate Data
            var aggregatedReadingDto = await aggregator.AggregateRawReading(sensorData, workCts.Token);
            var aggregate = mapper.Map<ReadingAggregate>(aggregatedReadingDto);

            // Store Aggregated Data

            await Task.WhenAll([
                readingRepository.CreateAsync(aggregate, workCts.Token),
                hourlyReadingService.ProcessReadingAsync(sensorData, workCts.Token),
            ]);
            
            // Notify Clients of Update
            await notifier.SendDashboardUpdateAsync(aggregatedReadingDto, workCts.Token);

            logger.LogInformation("Sensor data processed successfully.");
        }
        catch (OperationCanceledException operationCanceledException)
        {
            if (stoppingToken.IsCancellationRequested && operationCanceledException.CancellationToken == workCts.Token)
            {
                logger.LogInformation("Sensor data processing was cancelled by the stopping token.");
            }
            else if (operationCanceledException.CancellationToken == workCts.Token)
            {
                logger.LogWarning("Sensor data processing timed out after {Timeout} and was cancelled. Attempting to cancel and skip this iteration.", _processingTimeout);
            }
            else
            {
                logger.LogWarning(operationCanceledException, "Sensor data processing was cancelled due to an unexpected cancellation request.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while processing the sensor reading.");
            throw;
        }
        finally
        {
            logger.LogDebug("Finished attempt to process sensor data.");
        }
    }
    private async Task AttemptToConnectSensorClientAsync(CancellationToken cancellationToken)
    {
        var attemptNumber = 1;
        while (attemptNumber <= sensorSettings.MaxRetryCount && !cancellationToken.IsCancellationRequested)
        {
            try
            {
                logger.LogInformation("Attempting to connect to sensor client. Attempt: {Attempts}", attemptNumber);
                var status = await sensorClient.ConnectAsync(cancellationToken);
                if (!status)
                {
                    attemptNumber++;
                    logger.LogWarning("Sensor client connection failed. Retrying... Attempt: {Attempts}", attemptNumber);
                    await Task.Delay(MaxRetryDelayMs, cancellationToken); // Wait before retrying
                    continue;
                }

                logger.LogInformation("Sensor client connected successfully.");
                break;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                logger.LogInformation("SensorPollingWorker start operation was cancelled and has shutdown.");
                return; // Exit if the operation was canceled
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while trying to connect to the sensor client. Attempt {AttemptNumber} of {MaxAttempts}.", attemptNumber, sensorSettings.MaxRetryCount);
                return;
            }
        }
    }
    private async Task DelayUntilNextTickAsync(CancellationToken cancellationToken)
    {
        var delay = DateTimeProvider.Instance.MillisecondsTillTenSeconds();
        if (delay > 0)
        {
            logger.LogDebug("Delaying until next tick by {Delay} milliseconds.", delay);
            await Task.Delay((int)delay + sensorSettings.DelayCushion, cancellationToken);
        }
        else
        {
            logger.LogDebug("No delay needed, proceeding to next tick.");
        }
    }

}
