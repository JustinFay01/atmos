using System.Collections.Concurrent;

using Application.Models;
using Application.Rules;

namespace Tests.ApplicationTests;

public class OneMinuteAverageRuleTests : BaseTest<OneMinuteAverageRule>
{
    private readonly DateTime _now = new(2023, 1, 1, 1, 0, 0, DateTimeKind.Utc);

    #region Early Exit Conditions

    public OneMinuteAverageRuleTests()
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

    #region Average Calculation

    [Test]
    public async Task OneMinuteAverageRule_Integer_CalculatesCorrectOneMinuteAverage()
    {
        // Arrange
        var aggregate = new MetricAggregate
        {
            RecentReadings = new ConcurrentQueue<Metric>([
                new Metric { Timestamp = _now.AddSeconds(-50), Value = 10 },
                new Metric { Timestamp = _now.AddSeconds(-40), Value = 20 },
                new Metric { Timestamp = _now.AddSeconds(-30), Value = 30 },
                new Metric { Timestamp = _now.AddSeconds(-20), Value = 40 },
                new Metric { Timestamp = _now.AddSeconds(-10), Value = 50 },
                new Metric { Timestamp = _now, Value = 60 }
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
        var aggregate = new MetricAggregate
        {
            RecentReadings = new ConcurrentQueue<Metric>([
                new Metric { Timestamp = _now.AddSeconds(-50), Value = 10.1 },
                new Metric { Timestamp = _now.AddSeconds(-40), Value = 20.2 },
                new Metric { Timestamp = _now.AddSeconds(-30), Value = 30.3 },
                new Metric { Timestamp = _now.AddSeconds(-20), Value = 40.4 },
                new Metric { Timestamp = _now.AddSeconds(-10), Value = 50.5 },
                new Metric { Timestamp = _now, Value = 60.6 }
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
