using Domain.Entities;

namespace Domain.Interfaces;

/// <summary>
/// Defines the contract for writing Reading entities to a CSV-style log file.
/// 
/// The Presentation layer is responsible for selecting and providing the log path
/// (e.g. via configuration). The Infrastructure layer provides the implementation
/// of this contract.
/// 
/// Rules:
/// - Log path is supplied externally (not chosen by the writer).
/// - File name may be provided; if omitted, the default is the current date (yyyy-MM-dd).
/// - Each Reading is written as a comma-separated line, ending with a newline.
/// - Each line is prepended with the ReadingAggregate's timestamp (hh:mm:ss).
/// </summary>
public interface IReadingLogWriter
{
    /// <summary>
    /// Appends a Reading entry to the log file at the specified path.
    /// </summary>
    /// <param name="readingAggregate">The Reading entity to log.</param>
    /// <param name="logPath">The directory path where logs should be written.
    /// The Presentation layer manages this location.</param>
    /// <param name="fileName">The file name to use. If null, defaults to today's date (yyyy-MM-dd.csv).</param>
    /// <param name="cancellationToken">A token to allow the operation to be canceled.</param>
    Task AppendAsync(
        ReadingAggregate readingAggregate,
        string logPath,
        string? fileName = null,
        CancellationToken cancellationToken = default);
}
