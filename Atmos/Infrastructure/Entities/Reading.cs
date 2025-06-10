using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Entities;

public class Reading : BaseEntity
{
    /// <summary>
    /// Server-side timestamp of when the reading was taken. Indexed for performance. Stored in UTC.
    /// </summary>
    public DateTime TimeStamp { get; set; }
    
    public double Temperature { get; set; }
    
    public double DewPoint { get; set; }
    
    public double Humidity { get; set; }
}

public class ReadingConfiguration : BaseEntityConfiguration<Reading>
{
    public override void Configure(EntityTypeBuilder<Reading> builder)
    {
        base.Configure(builder);
        
        builder.Property(c => c.TimeStamp)
            .HasColumnType("timestamp")
            .HasDefaultValueSql("now()")
            .IsRequired();
    }
}