using Domain.Entities;
using Domain.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ReadingAggregateRepository : IReadingAggregateRepository
{
    private readonly AtmosContext _context;

    public ReadingAggregateRepository(AtmosContext context)
    {
        _context = context;
    }

    public async Task<List<ReadingAggregate>> GetAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default)
    {
       var readings = await _context.ReadingAggregates
           .Where(r => r.Timestamp >= from && r.Timestamp <= to)
           .OrderBy(r => r.Timestamp)
            .ToListAsync(cancellationToken);
       
         return readings;
    }

    public async Task CreateAsync(ReadingAggregate readingAggregate, CancellationToken cancellationToken = default)
    {
        await _context.ReadingAggregates.AddAsync(readingAggregate, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
