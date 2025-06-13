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
}
