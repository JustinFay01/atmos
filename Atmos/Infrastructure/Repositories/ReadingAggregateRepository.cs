using Domain.Entities;
using Domain.Interfaces;

namespace Infrastructure.Repositories;

public class ReadingAggregateRepository : IReadingAggregateRepository
{
    private readonly AtmosContext _context;

    public ReadingAggregateRepository(AtmosContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(ReadingAggregate readingAggregate, CancellationToken cancellationToken = default)
    {
        await _context.ReadingAggregates.AddAsync(readingAggregate, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
