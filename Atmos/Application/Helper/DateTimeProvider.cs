namespace Application.Helper;

public abstract class DateTimeProvider
{
    private static DateTimeProvider? _instance;

    public abstract DateTime Now { get; }
    public abstract DateTimeOffset UtcNow { get; }

    public static DateTimeProvider Instance
    {
        get => _instance ??= new DefaultDateTimeProvider();
        set => _instance = value;
    }
}

public class DefaultDateTimeProvider : DateTimeProvider
{
    public override DateTime Now => DateTime.Now;

    public override DateTimeOffset UtcNow => DateTimeOffset.UtcNow;

}

public static class TimeProviderExtensions
{
    public static double MillisecondsTillTenSeconds(this DateTimeProvider provider)
    {
        var secondsElapsed = provider.Now.Second + provider.Now.Millisecond / 1000.0;
        var remainder = secondsElapsed % 10;
        var millisecondsTillNextTen = (10 - remainder) * 1000;
        var rounded = Math.Round(millisecondsTillNextTen, 0);
        return rounded;
    }
}

