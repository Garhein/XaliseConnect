using Microsoft.Extensions.DependencyInjection;

namespace XaliseConnect.Application
{
    /// <summary>
    /// Provides extension methods for registering application
    /// services in the dependency injection container.
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Adds application services to the specified IServiceCollection.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Add application services here
            return services;
        }
    }
}
