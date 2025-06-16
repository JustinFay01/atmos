using Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public class ReadingAggregateConfiguration : BaseEntityConfiguration<ReadingAggregate>
{
    public override void Configure(EntityTypeBuilder<ReadingAggregate> builder)
    {
        base.Configure(builder);

        builder.Property(c => c.Timestamp)
            .HasColumnType("timestamptz")
            .HasDefaultValueSql("now()")
            .IsRequired();

    }
}
