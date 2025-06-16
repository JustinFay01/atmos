using Domain.Entities;

namespace Domain.Interfaces;

/// <summary>
/// Defines the contract for persistence operations related to the Reading entity.
/// This interface belongs to the Domain layer and is implemented by the Infrastructure layer.
/// </summary>
public interface IReadingAggregateRepository
{
    /// <summary>
    /// Fetches all Reading entities within the specified time range.
    /// </summary>
    /// <param name="from">The UTC timestamp to start from</param>
    /// <param name="to">The UTC timestamp to end with (inclusive)</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<ReadingAggregate>> GetAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default);
    /// <summary>
    /// Persists a new Reading entity to the data store.
    /// </summary>
    /// <param name="readingAggregate">The Reading entity to save. Note this is the domain entity, not a DTO.</param>
    /// <param name="cancellationToken">A token to allow the operation to be canceled.</param>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    Task CreateAsync(ReadingAggregate readingAggregate, CancellationToken cancellationToken = default);
}
