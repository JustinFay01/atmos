using System.Collections.Concurrent;

using Application.Models;
using Application.Rules;

namespace Tests.ApplicationTests;

public class MetricUpdateRuleTests : BaseTest<OneMinuteAverageRule>
{
    private readonly DateTime _now = new(2023, 1, 1, 1, 0, 0, DateTimeKind.Utc);

    #region Early Exit Conditions

    public MetricUpdateRuleTests()
    {
        Subject = new OneMinuteAverageRule(LoggerMock.Object);
    }

    [Test]
    public async Task OneMinuteAverageRule_ShouldReturnSameAggregate_WhenRecentReadingsIsEmpty()
    {
        // Arrange
        var aggregate = new MetricAggregate();
        var newMetric = new Metric();

        // Act
        var result = Subject.Apply(aggregate, newMetric);

        // Assert
        await Assert.That(result).IsEqualTo(aggregate);
    }

    [Test]
    public async Task OneMinuteAverageRule_ShouldReturnSameAggregate_WhenRecentReadingsCountIsLessThanMaxReadings()
    {
        // Arrange
        var aggregate = new MetricAggregate
        {
            RecentReadings = new ConcurrentQueue<Metric>([
                new Metric { Timestamp = _now, Value = 1 },
                new Metric { Timestamp = _now.AddSeconds(10), Value = 2 }
            ])
        };
        var newMetric = new Metric();

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
        var aggregate = new MetricAggregate
        {
            RecentReadings = new ConcurrentQueue<Metric>([
                new Metric { Timestamp = _now.AddSeconds(-50), Value = 1 },
                new Metric { Timestamp = _now.AddSeconds(-40), Value = 2 },
                new Metric { Timestamp = _now.AddSeconds(-30), Value = 3 },
                new Metric { Timestamp = _now.AddSeconds(-20), Value = 4 },
                new Metric { Timestamp = _now.AddSeconds(-10), Value = 5 },
                new Metric { Timestamp = _now, Value = 6 }
            ])
        };
        var newMetric = new Metric { Timestamp = _now, Value = 6 };

        // Act
        var result = Subject.Apply(aggregate, newMetric);

        // Assert
        await Assert.That(result.RecentReadings.Count).IsEqualTo(6);
        await Assert.That(result.OneMinuteAverages.Count).IsEqualTo(1);
    }

    [Test]
    public async Task OneMinuteAverageRule_ReturnsSameAggregate_WhenInMiddleOfMinute()
    {
        // Arrange
        var aggregate = new MetricAggregate
        {
            RecentReadings = new ConcurrentQueue<Metric>([
                new Metric { Timestamp = _now.AddSeconds(-40), Value = 1 },
                new Metric { Timestamp = _now.AddSeconds(-30), Value = 2 },
                new Metric { Timestamp = _now.AddSeconds(-20), Value = 3 },
                new Metric { Timestamp = _now.AddSeconds(-10), Value = 4 },
                new Metric { Timestamp = _now, Value = 5 },
                new Metric { Timestamp = _now.AddSeconds(10), Value = 6 }
            ])
        };

        // Act
        var result = Subject.Apply(aggregate, aggregate.RecentReadings.Last());

        // Assert
        await Assert.That(result.OneMinuteAverages.Count).IsEqualTo(0);
    }

    #endregion


}
