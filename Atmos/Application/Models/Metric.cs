namespace Application.Models;

public class Metric
{
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.MinValue;
    public double Value { get; init; } = double.MinValue;

    public override string ToString() => $"{Timestamp} - {Value}";
}
