namespace Application.Dtos;

public class MetricDto
{
    public DateTime Timestamp { get; init; } = DateTime.MinValue;
    public double Value { get; init; } = double.MinValue;
    public override string ToString() => $"{Timestamp} - {Value}";
}
