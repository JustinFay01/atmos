using Application.Helper;

using AutoFixture;

using Microsoft.Extensions.Logging;

using Moq;

namespace Tests.ApplicationTests;

public abstract class BaseTest<T>
    where T : class
{
    protected T Subject = null!;
    protected readonly Mock<ILogger<T>> LoggerMock = new();
    protected readonly Fixture Fixture = new();
    protected readonly Mock<DateTimeProvider> DateTimeProviderMock = new();

    public BaseTest()
    {
        DateTimeProviderMock.Setup(tp => tp.Now).Returns(
                new DateTime(2023, 1, 1, 1, 0, 0, DateTimeKind.Utc)
            );
    }
}
