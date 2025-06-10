using Domain.Entities;

namespace Domain.Interfaces;

/// <summary>
/// Defines the contract for persistence operations related to the Reading entity.
/// This interface belongs to the Domain layer and is implemented by the Infrastructure layer.
/// </summary>
public interface IReadingRepository
{
    /// <summary>
    /// Persists a new Reading entity to the data store.
    /// </summary>
    /// <param name="reading">The Reading entity to save. Note this is the domain entity, not a DTO.</param>
    /// <param name="cancellationToken">A token to allow the operation to be cancelled.</param>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    Task CreateAsync(Reading reading, CancellationToken cancellationToken = default);
}