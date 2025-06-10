using Infrastructure.Entities;

namespace Infrastructure.Repositories;

public interface IReadingRepository
{
    /// <summary>
    ///  Creates a new reading in the database.
    /// </summary>
    /// <param name="reading"></param>
    /// <returns></returns>
    Task CreateReadingAsync(Reading reading);
}

public class ReadingRepository : IReadingRepository
{
    private readonly AtmosContext _context;

    public ReadingRepository(AtmosContext context)
    {
        _context = context;
    }

    public async Task CreateReadingAsync(Reading reading)
    {
        await _context.Readings.AddAsync(reading);
        await _context.SaveChangesAsync();
    }
}