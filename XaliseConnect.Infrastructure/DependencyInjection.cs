using Microsoft.Extensions.DependencyInjection;

namespace XaliseConnect.Infrastructure
{
    /// <summary>
    /// Provides extension methods for registering infrastructure
    /// services in the dependency injection container.
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Adds infrastructure services to the specified IServiceCollection.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            // Add infrastructure services here
            return services;
        }
    }
}
