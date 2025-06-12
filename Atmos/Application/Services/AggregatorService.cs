using Application.Interfaces;
using Application.Models;
using Application.Rules;

using Domain.Entities;

using Microsoft.Extensions.Logging;

namespace Application.Services;

public class AggregatorService : IAggregator
{
    private readonly ILogger<AggregatorService> _logger;
    private readonly IRealtimeUpdateNotifier _notifier;

    // Concurrency control for each metric aggregate
    private readonly SemaphoreSlim _temperatureLock = new(1, 1);
    private readonly SemaphoreSlim _humidityLock = new(1, 1);
    private readonly SemaphoreSlim _dewPointLock = new(1, 1);

    // The order of rules matters, as they will be applied sequentially
    // Specifically, the OneMinuteRollingAverageRule must take place before the FiveMinuteRollingAverageRule 
    // to ensure that the five-minute average is calculated correctly based on the one-minute averages.
    private readonly List<IMetricUpdateRule> _rules;

    public AggregatorService(ILogger<AggregatorService> logger, IRealtimeUpdateNotifier notifier, OneMinuteAverageRule oneMinuteAverageRule)
    {
        _logger = logger;
        _notifier = notifier;

        _rules =
        [
            new CurrentValueRule(),
            new MaxRule(),
            new MinRule(),
            new RecentReadingsRule(),
            oneMinuteAverageRule,
        ];
    }

    public MetricAggregate Temperature { get; private set; } = new();
    public MetricAggregate Humidity { get; private set; } = new();
    public MetricAggregate DewPoint { get; private set; } = new();
    public async Task ProcessReadingAsync(Reading reading, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Processing reading: {reading}", reading);

        var newTemp = new Metric
        {
            Timestamp = reading.TimeStamp,
            Value = reading.Temperature
        };
        var newHumidity = new Metric
        {
            Timestamp = reading.TimeStamp,
            Value = reading.Humidity
        };
        var newDewPoint = new Metric
        {
            Timestamp = reading.TimeStamp,
            Value = reading.DewPoint
        };
        var tasks = new List<Task<MetricAggregate>>
        {
            ApplyRulesAsync(Temperature, newTemp, _temperatureLock, nameof(Temperature),cancellationToken),
            ApplyRulesAsync(Humidity, newHumidity, _humidityLock, nameof(Humidity), cancellationToken),
            ApplyRulesAsync(DewPoint, newDewPoint, _dewPointLock, nameof(DewPoint), cancellationToken)
        };

        var results = await Task.WhenAll(tasks);
        Temperature = results[0];
        Humidity = results[1];
        DewPoint = results[2];

        _logger.LogDebug("Updated aggregates: Temperature={Temperature}, Humidity={Humidity}, DewPoint={DewPoint}",
            Temperature, Humidity, DewPoint);

        // Notify subscribers about the updated aggregates
        await _notifier.SendDashboardUpdateAsync(new DashboardUpdate()
        {
            Temperature = Temperature,
            Humidity = Humidity,
            DewPoint = DewPoint,
            LatestReading = reading,
        }, cancellationToken);

    }

    private async Task<MetricAggregate> ApplyRulesAsync(MetricAggregate aggregate, Metric newValue, SemaphoreSlim locker, string metricName, CancellationToken cancellationToken = default)
    {
        _logger.LogTrace("Applying rules to {metricName}: {aggregate} with new value: {newValue}", metricName, aggregate, newValue);
        await locker.WaitAsync(cancellationToken);
        try
        {
            return _rules.Aggregate(aggregate, (current, rule) => rule.Apply(current, newValue));
        }
        finally
        {
            _logger.LogTrace("Finished applying rules to aggregate: {aggregate}", aggregate);
            locker.Release();
        }
    }
}
