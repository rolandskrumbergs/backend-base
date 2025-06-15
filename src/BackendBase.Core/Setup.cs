using System.Reflection;
using BackendBase.Core.Infrastructure.Mediator;
using Microsoft.Extensions.DependencyInjection;

namespace BackendBase.Core;

public static class Setup
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        services.AddScoped<IMediator, Mediator>();

        // Get all types from the Core assembly
        var assembly = Assembly.GetExecutingAssembly();
        var handlerTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && IsRequestHandler(t));

        foreach (var handlerType in handlerTypes)
        {
            var interfaces = handlerType.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>));

            foreach (var handlerInterface in interfaces)
            {
                services.AddScoped(handlerInterface, handlerType);
            }
        }

        return services;
    }

    private static bool IsRequestHandler(Type type)
    {
        return type.GetInterfaces()
            .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>));
    }
}