using System.Globalization;

using Domain.Entities;
using Domain.Interfaces;

using Microsoft.Extensions.Logging;

namespace Infrastructure.Logs;

public class CsvReadingLogWriter(ILogger<CsvReadingLogWriter> logger) : IReadingLogWriter
{
    public async Task AppendAsync(
        ReadingAggregate readingAggregate,
        string logPath,
        string? fileName = null,
        CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(logPath);

        // Default file name = today's date
        var actualFileName = string.IsNullOrWhiteSpace(fileName)
            ? $"{DateTime.UtcNow:yyyy-MM-dd}.csv"
            : fileName;

        var filePath = Path.Combine(logPath, actualFileName);

        try
        {
            await using var writer = new StreamWriter(filePath, append: true);

            // Format: hh:mm:ss,<value>
            var line = string.Format(
                CultureInfo.InvariantCulture,
                "{0:HH:mm:ss},{1}, {2}, {3}",
                readingAggregate.Timestamp.ToLocalTime(),
                readingAggregate.Temperature,
                readingAggregate.Humidity,
                readingAggregate.DewPoint);

            await writer.WriteLineAsync(line.AsMemory(), cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError("Error writing to file {FilePath}, {message}", filePath, ex.Message);
            // re-throw 
            throw;
        }
    }
}
