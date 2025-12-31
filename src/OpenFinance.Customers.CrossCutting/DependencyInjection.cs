using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenFinance.Customers.CrossCutting.Caching;
using OpenFinance.Customers.CrossCutting.Extensions;
using OpenFinance.Customers.CrossCutting.Logging;
using OpenFinance.Customers.CrossCutting.Persistence;
using OpenFinance.Customers.CrossCutting.Telemetry;

namespace OpenFinance.Customers.CrossCutting
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCrossCutting(
            this IServiceCollection services, 
            Assembly assembly,
            IConfiguration configuration)
        {
            services.AddCaching(configuration);
            services
               .AddHandlersFromAssembly(assembly)
               .DecorateHandlersWith(typeof(TransactionDecorator<,>))
               .DecorateHandlersWith(typeof(CachingDecorator<,>))
               .DecorateHandlersWith(typeof(PerformanceDecorator<,>))
               .DecorateHandlersWith(typeof(LoggingDecorator<,>));

            return services;
        }
    }
}