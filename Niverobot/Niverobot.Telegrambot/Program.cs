using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Niverobot.Domain;
using Niverobot.Domain.EfModels;
using Niverobot.Interfaces;
using Niverobot.Services;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Serilog;

namespace Niverobot.Telegrambot
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(LoadConfiguration())
                .WriteTo.Console()
                .CreateLogger();

            Log.Information("Starting up...");
            using (var host = CreateHostBuilder(args).Build())
            {
                host.StartAsync();
                var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();

                // insert other console app code here

                lifetime.StopApplication();
                host.WaitForShutdownAsync();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host
                .CreateDefaultBuilder(args)
                .UseConsoleLifetime()
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

                    services.AddScoped<IDadJokeService, DadJokeService>();

                    // Add the config to our DI container for later user
                    services.AddSingleton(config);

                    services.Configure<BotConfiguration>(config.GetSection("BotConfiguration"));

                    services.AddDbContext<NiveroBotContext>(options =>
                        options.UseMySql(config.GetConnectionString("SqlServer"), mySqlOptions => mySqlOptions
                            .ServerVersion(new Version(8, 0, 18), ServerType.MySql)
                        ));

                    services.AddInternalServices();
                    services.AddHostedService<DatabaseStartup>();
                    services.AddHostedService<Telegrambot>();
                });

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