using System;
using System.Timers;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Niverobot.Domain.EfModels;
using Niverobot.Interfaces;
using Serilog;

namespace Niverobot.Consumer
{
    public class Consumer
    {
        private readonly IConfiguration _config;
        private readonly IReminderService _reminderService;

        public Consumer(IConfiguration config, IReminderService reminderService)
        {
            _config = config;
            _reminderService = reminderService;
        }

        // Application starting point
        public void Run()
        {
            try
            {
                // 10 Seconds interval
                Timer timer = new Timer(10000);
                timer.Elapsed += ( sender, e ) =>  RetrieveAndSendReminders();
                timer.Start();
                
            }
            catch (Exception e)
            {
                Log.Error("Error consuming reminder {0}", e);
            }
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