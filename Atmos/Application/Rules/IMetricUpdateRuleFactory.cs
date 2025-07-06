using Application.Models;

using Microsoft.Extensions.Logging;

namespace Application.Rules;

public interface IMetricUpdateRuleFactory
{
    public IReadOnlyList<IMetricUpdateRule> CreateRules();
}

public class MetricUpdateRuleFactory(ILogger<OneMinuteAverageRule> oneMinuteAverageRuleLogger, SensorSettings sensorSettings)
    : IMetricUpdateRuleFactory
{
    public IReadOnlyList<IMetricUpdateRule> CreateRules()
    {
        return new List<IMetricUpdateRule>
        {
            new CurrentValueRule(),
            new MaxRule(),
            new MinRule(),
            new RecentReadingsRule(),
            new OneMinuteAverageRule(oneMinuteAverageRuleLogger, sensorSettings)
        };
    }
}
