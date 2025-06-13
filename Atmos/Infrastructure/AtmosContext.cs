using Domain.Entities;

using Infrastructure.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure;

public class AtmosContext : DbContext
{
    public DbSet<Reading> Readings { get; set; }

    public string DbPath { get; }

    public AtmosContext(DbContextOptions<AtmosContext> options) : base(options)
    {
        const Environment.SpecialFolder folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = Path.Combine(path, "atmos.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite($"Data Source={DbPath}");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new ReadingConfiguration());
    }


    /// <summary>
    /// This is needed for the migration commands to work in the design-time environment.
    /// </summary>
    public class AtmosContextFactory : IDesignTimeDbContextFactory<AtmosContext>
    {
        public AtmosContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AtmosContext>();
            const Environment.SpecialFolder folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            var dbPath = Path.Combine(path, "atmos.db");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");

            return new AtmosContext(optionsBuilder.Options);
        }
    }

}
