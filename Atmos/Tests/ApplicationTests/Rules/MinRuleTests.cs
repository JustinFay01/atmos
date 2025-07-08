using Application.Dtos;
using Application.Models;
using Application.Rules;

using AutoFixture;


namespace Tests.ApplicationTests.Rules;

public class MinRuleTests : BaseTest<MinRule>
{
    public MinRuleTests()
    {
        Subject = new MinRule();
    }

    [Test]
    public async Task MinRule_ResetsMin_WhenNewMetricIsDifferentDay()
    {
        // Arrange
        var oldMetric = Fixture.Build<MetricDto>()
            .With(m => m.Timestamp, new DateTime(2025, 6, 12, 23, 59, 59))
            .With(m => m.Value, -1)
            .Create();
        var newMetric = Fixture.Build<MetricDto>()
            .With(m => m.Timestamp, new DateTime(2025, 6, 13, 0, 0, 1))
            .With(m => m.Value, 1)
            .Create();
        var aggregate = Fixture.Build<SingleReadingAggregateDto>()
            .With(a => a.MinValue, oldMetric)
            .Create();

        // Act
        var result = Subject.Apply(aggregate, newMetric);

        // Assert
        await Assert.That(result.MinValue.Value).IsEqualTo(newMetric.Value);
    }

    [Test]
    public async Task MinRule_UpdatesMin_WhenNewMetricIsLower()
    {
        // Arrange
        var oldMetric = Fixture.Build<MetricDto>()
            .With(m => m.Timestamp, new DateTime(2025, 6, 12, 23, 59, 59))
            .With(m => m.Value, -1)
            .Create();
        var newMetric = Fixture.Build<MetricDto>()
            .With(m => m.Timestamp, new DateTime(2025, 6, 12, 23, 59, 59))
            .With(m => m.Value, -2)
            .Create();
        var aggregate = Fixture.Build<SingleReadingAggregateDto>()
            .With(a => a.MinValue, oldMetric)
            .Create();

        // Act
        var result = Subject.Apply(aggregate, newMetric);

        // Assert
        await Assert.That(result.MinValue.Value).IsEqualTo(newMetric.Value);
    }

    [Test]
    public async Task MinRule_DoesNotUpdateMin_WhenNewMetricIsHigher()
    {
        // Arrange
        var oldMetric = Fixture.Build<MetricDto>()
            .With(m => m.Timestamp, new DateTime(2025, 6, 12, 23, 59, 59))
            .With(m => m.Value, -1)
            .Create();
        var newMetric = Fixture.Build<MetricDto>()
            .With(m => m.Timestamp, new DateTime(2025, 6, 12, 23, 59, 59))
            .With(m => m.Value, 0)
            .Create();
        var aggregate = Fixture.Build<SingleReadingAggregateDto>()
            .With(a => a.MinValue, oldMetric)
            .Create();

        // Act
        var result = Subject.Apply(aggregate, newMetric);

        // Assert
        await Assert.That(result.MinValue.Value).IsEqualTo(oldMetric.Value);
    }

    [Test]
    public async Task MinRule_KeepsMin_WhenValuesAreEqualWithinEpsilon()
    {
        // Arrange
        var oldMetric = Fixture.Build<MetricDto>()
            .With(m => m.Timestamp, new DateTime(2025, 6, 12, 23, 59, 59))
            .With(m => m.Value, -1)
            .Create();
        var newMetric = Fixture.Build<MetricDto>()
            .With(m => m.Timestamp, new DateTime(2025, 6, 12, 23, 59, 59))
            .With(m => m.Value, -1 + float.Epsilon / 2) // Slightly higher than old metric
            .Create();
        var aggregate = Fixture.Build<SingleReadingAggregateDto>()
            .With(a => a.MinValue, oldMetric)
            .Create();

        // Act
        var result = Subject.Apply(aggregate, newMetric);

        // Assert
        await Assert.That(result.MinValue.Value).IsEqualTo(oldMetric.Value);
    }
}
