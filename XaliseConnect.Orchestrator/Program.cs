using XaliseConnect.Application;
using XaliseConnect.Infrastructure;

namespace XaliseConnect.Orchestrator
{
    public class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddApplication()
                            .AddInfrastructure()
                            .AddHostedService<Worker>();

            // Logging configuration
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();

            IHost host = builder.Build();
            host.Run();
        }
    }
}
