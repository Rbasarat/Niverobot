using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Niverobot.Domain.EfModels;

namespace Niverobot.Telegrambot
{
    public class DatabaseStartup : IHostedService {
        private readonly IServiceProvider _serviceProvider;
        public DatabaseStartup(IServiceProvider serviceProvider){
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<NiveroBotContext>();
                db.Database.Migrate();
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}