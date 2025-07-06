using System.Collections.Concurrent;

using Application.Dtos;
using Application.Models;
using Application.Rules;

namespace Tests.ApplicationTests.Rules;

public class OneMinuteAverageRuleTests : BaseTest<OneMinuteAverageRule>
{
    private readonly DateTime _now = new(2023, 1, 1, 1, 0, 0, DateTimeKind.Utc);
    private readonly SensorSettings _sensorSettings = new()
    {
        Tolerance = 4000, // 4 seconds in milliseconds
        WaitTillNearestTenSeconds = true,
        DelayCushion = 150,
        MaxRetryCount = 3
    };

    #region Early Exit Conditions

    public OneMinuteAverageRuleTests()
    {
        Subject = new OneMinuteAverageRule(LoggerMock.Object, _sensorSettings);
    }

    [Test]
    public async Task OneMinuteAverageRule_ShouldReturnSameAggregate_WhenRecentReadingsIsEmpty()
    {
        // Arrange
        var aggregate = new SingleReadingAggregateDto();
        var newMetric = new MetricDto();

        // Act
        var result = Subject.Apply(aggregate, newMetric);

        // Assert
        await Assert.That(result).IsEqualTo(aggregate);
    }

    [Test]
    public async Task OneMinuteAverageRule_ShouldReturnSameAggregate_WhenRecentReadingsCountIsLessThanMaxReadings()
    {
        // Arrange
        var aggregate = new SingleReadingAggregateDto
        {
            RecentReadings = new ConcurrentQueue<MetricDto>([
                new MetricDto { Timestamp = _now, Value = 1 },
                new MetricDto { Timestamp = _now.AddSeconds(10), Value = 2 }
            ])
        };
        var newMetric = new MetricDto();

        // Act
        var result = Subject.Apply(aggregate, newMetric);

        // Assert
        await Assert.That(result).IsEqualTo(aggregate);
    }

    #endregion

    #region Timing Validation

    [Test]
    public async Task OneMinuteAverageRule_ReturnsNewAggregate_WhenAtTopOfMinute()
    {
        // Arrange
        var aggregate = new SingleReadingAggregateDto
        {
            RecentReadings = new ConcurrentQueue<MetricDto>([
                new MetricDto { Timestamp = _now.AddSeconds(-50), Value = 1 },
                new MetricDto { Timestamp = _now.AddSeconds(-40), Value = 2 },
                new MetricDto { Timestamp = _now.AddSeconds(-30), Value = 3 },
                new MetricDto { Timestamp = _now.AddSeconds(-20), Value = 4 },
                new MetricDto { Timestamp = _now.AddSeconds(-10), Value = 5 },
                new MetricDto { Timestamp = _now, Value = 6 }
            ])
        };
        var newMetric = new MetricDto { Timestamp = _now, Value = 6 };

        // Act
        var result = Subject.Apply(aggregate, newMetric);

        // Assert
        await Assert.That(result.RecentReadings.Count).IsEqualTo(6);
        await Assert.That(result.OneMinuteAverages.Count).IsEqualTo(1);
    }

    [Test]
    public async Task OneMinuteAverageRule_ReturnsNewAggregate_WhenInMiddleOfMinute()
    {
        // Arrange
        var aggregate = new SingleReadingAggregateDto
        {
            RecentReadings = new ConcurrentQueue<MetricDto>([
                new MetricDto { Timestamp = _now.AddSeconds(-40), Value = 1 },
                new MetricDto { Timestamp = _now.AddSeconds(-30), Value = 2 },
                new MetricDto { Timestamp = _now.AddSeconds(-20), Value = 3 },
                new MetricDto { Timestamp = _now.AddSeconds(-10), Value = 4 },
                new MetricDto { Timestamp = _now, Value = 5 },
                new MetricDto { Timestamp = _now.AddSeconds(10), Value = 6 }
            ])
        };

        // Act
        var result = Subject.Apply(aggregate, aggregate.RecentReadings.Last());

        // Assert
        await Assert.That(result.OneMinuteAverages.Count).IsEqualTo(1);
    }

    #endregion

    #region Average Calculation

    [Test]
    public async Task OneMinuteAverageRule_Integer_CalculatesCorrectOneMinuteAverage()
    {
        // Arrange
        var aggregate = new SingleReadingAggregateDto
        {
            RecentReadings = new ConcurrentQueue<MetricDto>([
                new MetricDto { Timestamp = _now.AddSeconds(-50), Value = 10 },
                new MetricDto { Timestamp = _now.AddSeconds(-40), Value = 20 },
                new MetricDto { Timestamp = _now.AddSeconds(-30), Value = 30 },
                new MetricDto { Timestamp = _now.AddSeconds(-20), Value = 40 },
                new MetricDto { Timestamp = _now.AddSeconds(-10), Value = 50 },
                new MetricDto { Timestamp = _now, Value = 60 }
            ])
        };

        // Act
        var result = Subject.Apply(aggregate, aggregate.RecentReadings.Last());

        // Assert
        await Assert.That(result.OneMinuteAverages.Count).IsEqualTo(1);
        await Assert.That(result.OneMinuteAverages.First().Value).IsEqualTo(35); // Average of the values
    }

    [Test]
    public async Task OneMinuteAverageRule_Decimal_CalculatesCorrectOneMinuteAverage()
    {
        // Arrange
        var aggregate = new SingleReadingAggregateDto
        {
            RecentReadings = new ConcurrentQueue<MetricDto>([
                new MetricDto { Timestamp = _now.AddSeconds(-50), Value = 10.1 },
                new MetricDto { Timestamp = _now.AddSeconds(-40), Value = 20.2 },
                new MetricDto { Timestamp = _now.AddSeconds(-30), Value = 30.3 },
                new MetricDto { Timestamp = _now.AddSeconds(-20), Value = 40.4 },
                new MetricDto { Timestamp = _now.AddSeconds(-10), Value = 50.5 },
                new MetricDto { Timestamp = _now, Value = 60.6 }
            ])
        };

        // Act
        var result = Subject.Apply(aggregate, aggregate.RecentReadings.Last());

        // Assert
        await Assert.That(result.OneMinuteAverages.Count).IsEqualTo(1);
        await Assert.That(Math.Abs(result.OneMinuteAverages.First().Value - 35.35)).IsLessThan(float.Epsilon); // Average of the values
    }

    #endregion

}
