using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Niverobot.Interfaces;
using Serilog;

namespace Niverobot.Consumer
{
    public class Consumer : IHostedService, IDisposable
    {
        private readonly IConfiguration _config;
        private readonly IReminderService _reminderService;
        private Timer _timer;

        public Consumer(IConfiguration config, IReminderService reminderService)
        {
            _config = config;
            _reminderService = reminderService;
        }

        // Application starting point
        // public void Run()
        // {
        //     try
        //     {
        //         // 10 Seconds interval
        //         Timer timer = new Timer(10000);
        //         timer.Elapsed += (sender, e) => RetrieveAndSendReminders();
        //         timer.Start();
        //
        //     }
        //     catch (Exception e)
        //     {
        //         Log.Error("Error consuming reminder {0}", e);
        //     }
        // }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(RetrieveAndSendReminders, null, TimeSpan.Zero, 
                TimeSpan.FromSeconds(10));
            
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        { 
            Log.Information("Timed Hosted Service is stopping.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
        
        private void RetrieveAndSendReminders(object state = null)
        {
            Console.WriteLine("starting check {0}", DateTime.UtcNow);
            try
            {
                var start = DateTime.UtcNow;
                var end = DateTime.UtcNow.AddMinutes(1);
                var nextReminders = _reminderService.GetNotSendReminders(start).ToList();
                var currentReminders = nextReminders.Where(x =>
                    x.TriggerDate < end).ToList();
                foreach (var reminder in currentReminders)
                {
                    _reminderService.SendReminderAsync(reminder);
                    _reminderService.SetReminderSend(reminder.Id);
                    Log.Information("Send reminder with id {0}", reminder.Id);
                }
            }
            catch (Exception e)
            {
                Log.Error("Error consuming reminder {0}", e);
            }
        }
    }
}