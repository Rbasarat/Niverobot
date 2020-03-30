using System;
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
            var startTimeSpan = TimeSpan.Zero;
            var periodTimeSpan = TimeSpan.FromMinutes(1);

            var timer = new System.Threading.Timer((e) =>
            {
                var now = DateTime.Now;
                var nextReminders = _reminderService.GetReminders(now);
                var currentReminders = nextReminders.Where(x => x.TriggerDate > now && x.TriggerDate < DateTime.Now.AddMinutes(1));
                foreach (var reminder in currentReminders)
                {
                    _reminderService.SendReminderAsync(reminder);
                }

            }, null, startTimeSpan, periodTimeSpan);
        }
    }
}