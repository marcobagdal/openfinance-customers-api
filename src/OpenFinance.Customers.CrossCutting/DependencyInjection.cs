using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace OpenFinance.Customers.CrossCutting
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCrossCutting(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Aqui vamos registrar todos os serviços e decorators
            // Cada módulo terá seu próprio método de extensão
            
            // Exemplo:
            // services.AddLogging(configuration);
            // services.AddCaching(configuration);
            // services.AddAuth(configuration);
            // services.AddTelemetry(configuration);
            
            return services;
        }
    }
}