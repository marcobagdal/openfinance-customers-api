using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;
using OpenFinance.Customers.SharedKernel.Common.Interfaces;

namespace OpenFinance.Customers.CrossCutting.Extensions;

public static class DecoratorExtensions
{
    /// <summary>
    /// PASSO 1: Escaneia e registra todos os Handlers concretos de um Assembly.
    /// </summary>
    public static IServiceCollection AddHandlersFromAssembly(this IServiceCollection services, Assembly assembly)
    {
        var handlerTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract &&
                t.GetInterfaces().Any(i => 
                    i.IsGenericType && 
                    i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)));

        foreach (var handlerType in handlerTypes)
        {
            var interfaceType = handlerType.GetInterfaces()
                .First(i => i.IsGenericType && 
                    i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>));
            
            // Registra a implementação base (o "coração" da cebola)
            services.TryAddScoped(interfaceType, handlerType);
        }

        return services;
    }

    /// <summary>
    /// PASSO 2: Envolve os Handlers já registrados com um Decorator.
    /// Pode ser chamado múltiplas vezes para criar camadas (cebola).
    /// </summary>
    public static IServiceCollection DecorateHandlersWith(this IServiceCollection services, Type decoratorType)
    {
        // Filtra os serviços que implementam IRequestHandler<,>
        var descriptors = services
            .Where(s => s.ServiceType.IsGenericType && 
                        s.ServiceType.GetGenericTypeDefinition() == typeof(IRequestHandler<,>))
            .ToList();

        foreach (var descriptor in descriptors)
        {
            var serviceType = descriptor.ServiceType;
            var genericArgs = serviceType.GetGenericArguments();
            
            // Fecha o tipo do decorator (ex: LoggingDecorator<Request, Response>)
            var closedDecoratorType = decoratorType.MakeGenericType(genericArgs);

            // Substitui o registro atual por uma nova "casca"
            services.Replace(ServiceDescriptor.Describe(
                serviceType,
                provider =>
                {
                    // Resolve a instância que está atualmente no DI (pode ser o Handler ou outro Decorator)
                    var inner = provider.CreateInstance(descriptor);
                    
                    // Cria o novo Decorator injetando a instância interna (Chain of Responsibility)
                    return ActivatorUtilities.CreateInstance(provider, closedDecoratorType, inner);
                },
                descriptor.Lifetime));
        }

        return services;
    }

    /// <summary>
    /// Helper de alta performance para resolver a instância interna do Descriptor.
    /// </summary>
    private static object CreateInstance(this IServiceProvider provider, ServiceDescriptor descriptor)
    {
        if (descriptor.ImplementationInstance != null)
            return descriptor.ImplementationInstance;

        if (descriptor.ImplementationFactory != null)
            return descriptor.ImplementationFactory(provider);

        return ActivatorUtilities.CreateInstance(provider, descriptor.ImplementationType!);
    }

}