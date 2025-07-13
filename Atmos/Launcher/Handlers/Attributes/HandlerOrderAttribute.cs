namespace Launcher.Handlers.Attributes;

public enum ChainType
{
    Install,
    Update
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class HandlerOrderAttribute : Attribute
{
    public ChainType Chain { get; }
    public int Order { get; }

    public HandlerOrderAttribute(ChainType chain, int order)
    {
        Chain = chain;
        Order = order;
    }
}
