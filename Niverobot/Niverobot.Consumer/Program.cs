using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Niverobot.Domain;
using Niverobot.Domain.EfModels;
using Niverobot.WebApi;
using Serilog;

namespace Niverobot.Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(LoadConfiguration())
                .WriteTo.Console()
                .CreateLogger();
            
            var builder = new HostBuilder()
                .UseSerilog()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                    config.AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true);
                    config.AddEnvironmentVariables();

                    if (args != null)
                    {
                        config.AddCommandLine(args);
                    }
                })
                .ConfigureServices((hostContext, services) =>
                {
                    // Set up the objects we need to get to configuration settings
                    var config = LoadConfiguration();

                    // Set up logger.
                    Log.Logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(config)
                        .CreateLogger();


                    // Add the config to our DI container for later user
                    services.AddSingleton(config);

                    services.Configure<BotConfiguration>(config.GetSection("BotConfiguration"));

                    services.AddDbContext<NiveroBotContext>(options =>
                        options.UseSqlServer(config.GetConnectionString("SqlServer")));

                    services.AddInternalServices();
                    services.AddHostedService<Consumer>();
                });

            builder.StartAsync();
        }

        // static void Main(string[] args)
        // {
        //     // Create service collection and configure our services
        //     var services = ConfigureServices();
        //     // Generate a provider
        //     var serviceProvider = services.BuildServiceProvider();
        //     try
        //     {
        //         // Kick off our actual code
        //         serviceProvider.GetService<Consumer>().Run();
        //         Console.ReadKey();
        //     }
        //     finally
        //     {
        //         // Flush logs when application is finished.
        //         Log.CloseAndFlush();
        //     }
        // }

        private static IServiceCollection ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();
            // Set up the objects we need to get to configuration settings
            var config = LoadConfiguration();

            // Set up logger.
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .WriteTo.Console()
                .CreateLogger();


            // Add the config to our DI container for later user
            services.AddSingleton(config);

            services.Configure<BotConfiguration>(config.GetSection("BotConfiguration"));

            services.AddDbContext<NiveroBotContext>(options =>
                options.UseSqlServer(config.GetConnectionString("SqlServer")));

            services.AddInternalServices();

            // IMPORTANT! Register our application entry point
            services.AddTransient<Consumer>();

            return services;
        }

        private static IConfiguration LoadConfiguration()

        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            return builder.Build();
        }
    }
}