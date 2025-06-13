using Application.DTOs;
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
    private readonly Mock<IRealtimeUpdateNotifier> _notifierMock;
    private readonly AggregatorService _aggregatorService;

    public AggregatorTests()
    {
        _notifierMock = new Mock<IRealtimeUpdateNotifier>();
        var oneMinuteRuleAverageLogger = new Mock<ILogger<OneMinuteAverageRule>>();
        var ruleFactory = new MetricUpdateRuleFactory(oneMinuteRuleAverageLogger.Object);

        _aggregatorService = new AggregatorService(
            LoggerMock.Object,
            _notifierMock.Object,
            ruleFactory
        );
    }

    [Test]
    public async Task AggregatorService_UpdateAggregates()
    {
        var reading = Fixture.Create<ReadingDto>();

        await _aggregatorService.ProcessReadingAsync(reading);

        await Assert.That(reading.Temperature).IsEqualTo(_aggregatorService.Temperature.CurrentValue.Value);
        await Assert.That(reading.Humidity).IsEqualTo(_aggregatorService.Humidity.CurrentValue.Value);
        await Assert.That(reading.DewPoint).IsEqualTo(_aggregatorService.DewPoint.CurrentValue.Value);
    }

    [Test]
    public async Task AggregatorService_NotifySubscribers()
    {
        var reading = Fixture.Create<ReadingDto>();

        await _aggregatorService.ProcessReadingAsync(reading);

        _notifierMock.Verify(n => n.SendDashboardUpdateAsync(It.IsAny<DashboardUpdate>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
