namespace Application.Extensions;

public static class DateTimeExtensions
{
    public static double MillisecondsTillTenSeconds(this DateTime dateTime)
    {
        var secondsElapsed = dateTime.Second + dateTime.Millisecond / 1000.0;
        var remainder = secondsElapsed % 10;
        var millisecondsTillNextTen = (10 - remainder) * 1000;
        var rounded = Math.Round(millisecondsTillNextTen, 0);
        return rounded;
    }
}
