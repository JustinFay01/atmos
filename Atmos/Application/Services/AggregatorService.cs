using Application.Dtos;
using Application.Interfaces;
using Application.Models;
using Application.Rules;

using Microsoft.Extensions.Logging;

namespace Application.Services;

public class AggregatorService(
    ILogger<AggregatorService> logger,
    IMetricUpdateRuleFactory ruleFactory)
    : IAggregator
{
    // Concurrency control for each metric aggregate
    private readonly SemaphoreSlim _temperatureLock = new(1, 1);
    private readonly SemaphoreSlim _humidityLock = new(1, 1);
    private readonly SemaphoreSlim _dewPointLock = new(1, 1);

    // The order of rules matters, as they will be applied sequentially
    // Specifically, the OneMinuteRollingAverageRule must take place before the FiveMinuteRollingAverageRule 
    // to ensure that the five-minute average is calculated correctly based on the one-minute averages.
    private readonly IReadOnlyCollection<IMetricUpdateRule> _rules = ruleFactory.CreateRules();

    private SingleReadingAggregateDto Temperature { get; set; } = new();
    private SingleReadingAggregateDto Humidity { get; set; } = new();
    private SingleReadingAggregateDto DewPoint { get; set; } = new();
    public ReadingAggregateDto? AggregatedReading { get; private set; }

    public async Task<ReadingAggregateDto> AggregateRawReading(RawSensorReading reading, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Processing reading: {reading}", reading);

        var newTemp = new MetricDto
        {
            Timestamp = reading.Timestamp,
            Value = reading.Temperature
        };
        var newHumidity = new MetricDto
        {
            Timestamp = reading.Timestamp,
            Value = reading.Humidity
        };
        var newDewPoint = new MetricDto
        {
            Timestamp = reading.Timestamp,
            Value = reading.DewPoint
        };
        var tasks = new List<Task<SingleReadingAggregateDto>>
        {
            ApplyRulesAsync(Temperature, newTemp, _temperatureLock, nameof(Temperature),cancellationToken),
            ApplyRulesAsync(Humidity, newHumidity, _humidityLock, nameof(Humidity), cancellationToken),
            ApplyRulesAsync(DewPoint, newDewPoint, _dewPointLock, nameof(DewPoint), cancellationToken)
        };

        var results = await Task.WhenAll(tasks);
        Temperature = results[0];
        Humidity = results[1];
        DewPoint = results[2];

        logger.LogDebug("Updated aggregates: Temperature={Temperature}, Humidity={Humidity}, DewPoint={DewPoint}",
            Temperature, Humidity, DewPoint);

        AggregatedReading = new ReadingAggregateDto
        {
            Temperature = Temperature,
            Humidity = Humidity,
            DewPoint = DewPoint,
            LatestReading = reading,
        };
        return AggregatedReading;
    }

    private async Task<SingleReadingAggregateDto> ApplyRulesAsync(SingleReadingAggregateDto aggregateDto, MetricDto newValue, SemaphoreSlim locker, string metricName, CancellationToken cancellationToken = default)
    {
        logger.LogTrace("Applying rules to {metricName}: {aggregate} with new value: {newValue}", metricName, aggregateDto, newValue);
        await locker.WaitAsync(cancellationToken);
        try
        {
            return _rules.Aggregate(aggregateDto, (current, rule) => rule.Apply(current, newValue));
        }
        finally
        {
            logger.LogTrace("Finished applying rules to aggregate: {aggregate}", aggregateDto);
            locker.Release();
        }
    }
}
