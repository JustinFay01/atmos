using System.Collections.Concurrent;

using Application.Helper;
using Application.Models;
using Application.Rules;

using Moq;

namespace Tests.ApplicationTests.Rules;

public class RecentReadingsRuleTests : BaseTest<RecentReadingsRule>
{
    private readonly Mock<DateTimeProvider> _timeProviderMock;
    private const int MaxReadings = 6; // Assuming we want to keep the last six readings

    public RecentReadingsRuleTests()
    {
        _timeProviderMock = new Mock<DateTimeProvider>();
        Subject = new RecentReadingsRule();

        _timeProviderMock.Setup(tp => tp.Now)
            .Returns(new DateTime(2023, 1, 1, 1, 0, 0, DateTimeKind.Utc));
    }

    [Test]
    public async Task RecentReadingsRule_ShouldUpdateRecentReadings_WhenNewMetricIsAdded()
    {
        // Arrange
        var aggregate = new MetricAggregate
        {
            RecentReadings = new ConcurrentQueue<Metric>([
                new Metric { Timestamp = _timeProviderMock.Object.Now, Value = 1 },
                new Metric { Timestamp = _timeProviderMock.Object.Now.AddSeconds(10), Value = 2 }
            ])
        };
        var newMetric = new Metric { Timestamp = _timeProviderMock.Object.Now.AddSeconds(20), Value = 3 };

        // Act
        var result = Subject.Apply(aggregate, newMetric);

        // Assert
        await Assert.That(result.RecentReadings.Count).IsEqualTo(3);

    }

    [Test]
    public async Task RecentReadingsRule_ShouldLimitRecentReadingsToMaxCount()
    {
        // Arrange
        var aggregate = new MetricAggregate
        {
            RecentReadings = new ConcurrentQueue<Metric>([
                new Metric { Timestamp = _timeProviderMock.Object.Now, Value = 1 },
                new Metric { Timestamp = _timeProviderMock.Object.Now.AddSeconds(10), Value = 2 },
                new Metric { Timestamp = _timeProviderMock.Object.Now.AddSeconds(20), Value = 3 },
                new Metric { Timestamp = _timeProviderMock.Object.Now.AddSeconds(30), Value = 4 },
                new Metric { Timestamp = _timeProviderMock.Object.Now.AddSeconds(40), Value = 5 },
                new Metric { Timestamp = _timeProviderMock.Object.Now.AddSeconds(50), Value = 6 }
            ])
        };
        var newMetric = new Metric { Timestamp = _timeProviderMock.Object.Now.AddSeconds(60), Value = 7 };

        // Act
        var result = Subject.Apply(aggregate, newMetric);

        // Assert
        await Assert.That(result.RecentReadings.Count).IsEqualTo(MaxReadings);
    }

}
