using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Niverobot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var services = ConfigureServices();
            // Generate a provider
            var serviceProvider = services.BuildServiceProvider();
            try
            {
                // Kick off our actual code
                await serviceProvider.GetService<ConsoleApplication>().RunAsync();
            }
            finally
            {
                // Flush logs when application is finished.
                Log.CloseAndFlush();
            }
        }
        private static IServiceCollection ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();
            // Set up the objects we need to get to configuration settings
            var config = LoadConfiguration();

            // Set up logger.
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .CreateLogger();

            // Add the config to our DI container for later user
            services.AddSingleton(config);
            // IMPORTANT! Register our application entry point
            services.AddTransient<ConsoleApplication>();

            return services;
        }

        public static IConfiguration LoadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            return builder.Build();
        }
    }
}
