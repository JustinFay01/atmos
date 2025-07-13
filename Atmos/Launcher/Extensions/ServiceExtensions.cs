using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace Launcher.Extensions;

public static class ServiceExtensions
 {
     public static IServiceCollection RegisterHandlers(this IServiceCollection services)
     {
         var handlerTypes = Assembly.GetCallingAssembly().GetTypes()
             .Where(t => typeof(IHandler).IsAssignableFrom(t) && t is { IsInterface: false, IsAbstract: false });

         foreach (var type in handlerTypes)
         {
             services.AddTransient(type);
         }

         return services;
     }
 }
