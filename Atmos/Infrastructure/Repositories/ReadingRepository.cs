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

    public async Task CreateAsync(Reading reading, CancellationToken cancellationToken = default)
    {
        await _context.Readings.AddAsync(reading, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
