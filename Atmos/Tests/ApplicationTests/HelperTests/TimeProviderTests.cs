using Application.Helper;

using Moq;

namespace Tests.ApplicationTests.HelperTests;

public class TimeProviderTests
{
    [Test]
    [Arguments(0, 0, 10000)]
    [Arguments(9, 0, 1000)]
    [Arguments(10, 0, 10000)]
    [Arguments(19, 0, 1000)]
    [Arguments(29, 0, 1000)]
    [Arguments(30, 0, 10000)]
    [Arguments(9, 500, 500)]      // Halfway through to 10
    [Arguments(59, 999, 1)]       // Edge case, just before 0 again
    [Arguments(10, 250, 9750)]
    public async Task MillisecondsTillTenSeconds_ReturnsExpectedValue(int second, int millisecond, double expectedMilliseconds)
    {
        // Arrange
        var timeProvider = new Mock<DateTimeProvider>();
        var dateTime = new DateTime(2025, 1, 1, 0, 0, second, millisecond);
        timeProvider.Setup(tp => tp.Now).Returns(dateTime);

        // Act
        var result = timeProvider.Object.MillisecondsTillTenSeconds();

        // Assert
        await Assert.That(result).IsEqualTo(expectedMilliseconds);
    }
}
