using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Entities;

public interface IBaseEntity<T>
{
    T Id { get; set; }
}

public abstract class BaseEntity : IBaseEntity<Guid>
{
    public Guid Id { get; set; }
}

public abstract class BaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.Property(c => c.Id)
            .HasColumnType("uuid")
            .ValueGeneratedOnAdd()
            .IsRequired();
    }
}
