using System;
using System.Linq;
using System.Threading;
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
                var timer = new Timer(RetrieveAndSendReminders, null, TimeSpan.Zero,
                    TimeSpan.FromSeconds(5));
            }
            catch (Exception e)
            {
                Log.Error("Error consuming reminder", e);
            }
        }

        private void RetrieveAndSendReminders(object state = null)
        {
            try
            {
                var now = DateTime.UtcNow;
                var nextReminders = _reminderService.GetReminders(now);
                var currentReminders = nextReminders.Where(x =>
                    x.TriggerDate > now && x.TriggerDate < now.AddMinutes(1));
                foreach (var reminder in currentReminders)
                {
                    _reminderService.SendReminderAsync(reminder);
                    _reminderService.SetReminderSend(reminder.Id);
                    Log.Information("Send reminder with id {0}", reminder.Id);
                }
            }
            catch (Exception e)
            {
                Log.Error("Error consuming reminder", e);
            }
        }
    }
}