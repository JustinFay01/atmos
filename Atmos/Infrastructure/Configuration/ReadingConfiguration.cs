using Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

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
