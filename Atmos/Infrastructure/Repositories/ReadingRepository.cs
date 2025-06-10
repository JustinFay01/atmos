using Domain.Entities;
using Domain.Interfaces;

namespace Infrastructure.Repositories;

public class ReadingRepository : IReadingRepository
{
    private readonly AtmosContext _context;

    public ReadingRepository(AtmosContext context)
    {
        _context = context;
    }

    public Task CreateAsync(Reading reading, CancellationToken cancellationToken = default)
    {
        if (reading == null)
        {
            throw new ArgumentNullException(nameof(reading), "Reading cannot be null");
        }

        _context.Readings.Add(reading);
        return _context.SaveChangesAsync(cancellationToken);
    }
}