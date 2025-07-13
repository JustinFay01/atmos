using Domain.Entities;

using Infrastructure.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure;

public class AtmosContext : DbContext
{
    public DbSet<ReadingAggregate> ReadingAggregates { get; set; }

    public AtmosContext(DbContextOptions<AtmosContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new ReadingAggregateConfiguration());
    }


    /// <summary>
    /// This is needed for the migration commands to work in the design-time environment.
    /// </summary>
    public class AtmosContextFactory : IDesignTimeDbContextFactory<AtmosContext>
    {
        public AtmosContext CreateDbContext(string[] args)
        {
            const string connectionStringEnvVar = "ATMOS_DB_CONNECTION";

            var connectionString = Environment.GetEnvironmentVariable(connectionStringEnvVar) ??
                                   "Host=localhost;Database=postgres;Username=postgres;Password=postgres";
            
            var optionsBuilder = new DbContextOptionsBuilder<AtmosContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new AtmosContext(optionsBuilder.Options);
        }
    }

}
