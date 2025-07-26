using System.Reflection;

using Launcher.Handlers.Attributes;

using Microsoft.Extensions.DependencyInjection;

namespace Launcher.Services;

public class ChainBuilder
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Assembly _assemblyToScan;
    public ChainBuilder(IServiceProvider serviceProvider, Assembly? assemblyToScan = null)
    {
        _serviceProvider = serviceProvider;
        _assemblyToScan = assemblyToScan ?? Assembly.GetExecutingAssembly();
    }

    public IHandler BuildChain(ChainType chainType)
    {
        var handlerTypes = _assemblyToScan.GetTypes()
            .Where(t => typeof(IHandler).IsAssignableFrom(t) && t is { IsInterface: false, IsAbstract: false });

        var orderedHandlers = handlerTypes
            .Select(t => new
            {
                HandlerType = t,
                Attribute = t.GetCustomAttributes<HandlerOrderAttribute>(inherit: false)
                    .FirstOrDefault(attr => attr.Chain == chainType)
            })
            .Where(x => x.Attribute != null)
            .OrderBy(x => x.Attribute!.Order)
            .Select(x => (IHandler)_serviceProvider.GetRequiredService(x.HandlerType))
            .ToList();

        if (orderedHandlers.Count == 0)
        {
            throw new InvalidOperationException($"No handlers found for chain type {chainType}");
        }
        
        for (var i = 0; i < orderedHandlers.Count - 1; i++)
        {
            orderedHandlers[i].SetNext(orderedHandlers[i + 1]);
        }
        return orderedHandlers.First();
    }
}
