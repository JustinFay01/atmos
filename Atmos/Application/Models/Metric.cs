namespace Application.Models;

public class Metric
{
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.MinValue;
    public double Value { get; init; } = double.MinValue;

    public Metric CopyWith(
        DateTimeOffset? timestamp = null,
        double? value = null)
    {
        return new Metric
        {
            Timestamp = timestamp ?? Timestamp,
            Value = value ?? Value
        };
    }
}
