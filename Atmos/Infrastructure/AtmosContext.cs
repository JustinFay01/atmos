using Domain.Entities;

using Infrastructure.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure;

public class AtmosContext : DbContext
{
    public DbSet<Reading> Readings { get; set; }

    public AtmosContext(DbContextOptions<AtmosContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReadingConfiguration).Assembly);
    }


    /// <summary>
    /// This is needed for the migration commands to work in the design-time environment.
    /// </summary>
    public class AtmosContextFactory : IDesignTimeDbContextFactory<AtmosContext>
    {
        public AtmosContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AtmosContext>();
            optionsBuilder.UseNpgsql("Host=localhost;Database=postgres;Username=postgres;Password=postgres");

            return new AtmosContext(optionsBuilder.Options);
        }
    }

}
