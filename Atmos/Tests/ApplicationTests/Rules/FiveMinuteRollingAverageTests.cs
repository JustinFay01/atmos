using System.Collections.Concurrent;

using Application.Dtos;

namespace Tests.ApplicationTests.Rules;

public class FiveMinuteRollingAverageTests 
{

    private readonly DateTime _now = new(2023, 1, 1, 1, 0, 0, DateTimeKind.Utc);


    [Test]
    public async Task FiveMinuteRollingAverage_IsNotNull_WhenOneMinuteAverageArrayIsFull()
    {
        var aggregate = new SingleReadingAggregateDto
        {
            OneMinuteAverages = new ConcurrentQueue<MetricDto>([
                new MetricDto { Timestamp = _now.AddSeconds(-50), Value = 10 },
                new MetricDto { Timestamp = _now.AddSeconds(-40), Value = 20 },
                new MetricDto { Timestamp = _now.AddSeconds(-30), Value = 30 },
                new MetricDto { Timestamp = _now.AddSeconds(-20), Value = 40 },
                new MetricDto { Timestamp = _now.AddSeconds(-10), Value = 50 },
            ])
        };

        // Act
        // Its a computed property, so we just access it
        
        // Assert
        await Assert.That(aggregate.FiveMinuteRollingAverage).IsNotNull();
    }
}
