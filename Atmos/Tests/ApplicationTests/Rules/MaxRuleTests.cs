using Application.Dtos;
using Application.Models;
using Application.Rules;

using AutoFixture;

namespace Tests.ApplicationTests.Rules;

public class MaxRuleTests : BaseTest<MaxRule>
{
    public MaxRuleTests()
    {
        Subject = new MaxRule();
    }

    [Test]
    public async Task MaxRule_ResetsMax_WhenNewMetricIsDifferentDay()
    {
        // Arrange
        var oldMetric = Fixture.Build<MetricDto>()
            .With(m => m.Timestamp, new DateTime(2025, 6, 12, 23, 59, 59))
            .With(m => m.Value, 1)
            .Create();
        var newMetric = Fixture.Build<MetricDto>()
            .With(m => m.Timestamp, new DateTime(2025, 6, 13, 0, 0, 1))
            .With(m => m.Value, -1)
            .Create();
        var aggregate = Fixture.Build<SingleReadingAggregateDto>()
            .With(a => a.MaxValue, oldMetric)
            .Create();

        // Act
        var result = Subject.Apply(aggregate, newMetric);

        // Assert
        await Assert.That(result.MaxValue.Value).IsEqualTo(newMetric.Value);
    }

    [Test]
    public async Task MaxRule_UpdatesMax_WhenNewMetricIsHigher()
    {
        // Arrange
        var oldMetric = Fixture.Build<MetricDto>()
            .With(m => m.Timestamp, new DateTime(2025, 6, 12, 10, 0, 0))
            .With(m => m.Value, 1)
            .Create();
        var newMetric = Fixture.Build<MetricDto>()
            .With(m => m.Timestamp, new DateTime(2025, 6, 12, 12, 0, 0))
            .With(m => m.Value, 2)
            .Create();
        var aggregate = Fixture.Build<SingleReadingAggregateDto>()
            .With(a => a.MaxValue, oldMetric)
            .Create();

        // Act
        var result = Subject.Apply(aggregate, newMetric);

        // Assert
        await Assert.That(result.MaxValue.Value).IsEqualTo(newMetric.Value);
    }

    [Test]
    public async Task MaxRule_DoesNotUpdateMax_WhenNewMetricIsLower()
    {
        // Arrange
        var oldMetric = Fixture.Build<MetricDto>()
            .With(m => m.Timestamp, new DateTime(2025, 6, 12, 10, 0, 0))
            .With(m => m.Value, 3)
            .Create();
        var newMetric = Fixture.Build<MetricDto>()
            .With(m => m.Timestamp, new DateTime(2025, 6, 12, 12, 0, 0))
            .With(m => m.Value, 2)
            .Create();
        var aggregate = Fixture.Build<SingleReadingAggregateDto>()
            .With(a => a.MaxValue, oldMetric)
            .Create();

        // Act
        var result = Subject.Apply(aggregate, newMetric);

        // Assert
        await Assert.That(result.MaxValue.Value).IsEqualTo(oldMetric.Value);
    }

    [Test]
    public async Task MaxRule_KeepsMax_WhenValuesAreEqualWithinEpsilon()
    {
        // Arrange
        var oldMetric = Fixture.Build<MetricDto>()
            .With(m => m.Timestamp, new DateTime(2025, 6, 12, 10, 0, 0))
            .With(m => m.Value, 3.0f)
            .Create();
        var newMetric = Fixture.Build<MetricDto>()
            .With(m => m.Timestamp, new DateTime(2025, 6, 12, 12, 0, 0))
            .With(m => m.Value, 3.0f - float.Epsilon / 2) // Slightly lower
            .Create();
        var aggregate = Fixture.Build<SingleReadingAggregateDto>()
            .With(a => a.MaxValue, oldMetric)
            .Create();

        // Act
        var result = Subject.Apply(aggregate, newMetric);

        // Assert
        await Assert.That(result.MaxValue.Value).IsEqualTo(oldMetric.Value);
    }
}
