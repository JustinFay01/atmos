using Application.Interfaces;
using Application.Models;
using Application.Rules;
using Application.Services;

using AutoFixture;

using Microsoft.Extensions.Logging;

using Moq;

namespace Tests.ApplicationTests.ServiceTests;

public class AggregatorTests : BaseTest<AggregatorService>
{
    private readonly AggregatorService _aggregatorService;

    public AggregatorTests()
    {
        var oneMinuteRuleAverageLogger = new Mock<ILogger<OneMinuteAverageRule>>();
        var ruleFactory = new MetricUpdateRuleFactory(oneMinuteRuleAverageLogger.Object);

        _aggregatorService = new AggregatorService(
            LoggerMock.Object,
            ruleFactory
        );
    }

    [Test]
    public async Task AggregatorService_UpdateAggregates()
    {
        var reading = Fixture.Create<RawSensorReading>();

        var aggregateDto = await _aggregatorService.AggregateRawReading(reading);

        await Assert.That(reading.Temperature).IsEqualTo(aggregateDto.Temperature.CurrentValue.Value);
        await Assert.That(reading.Humidity).IsEqualTo(aggregateDto.Humidity.CurrentValue.Value);
        await Assert.That(reading.DewPoint).IsEqualTo(aggregateDto.DewPoint.CurrentValue.Value);
    }
}
