namespace Domain.Entities;
public interface IBaseEntity<T>
{
    T Id { get; set; }
}

public abstract class BaseEntity : IBaseEntity<Guid>
{
    public Guid Id { get; set; }
}
