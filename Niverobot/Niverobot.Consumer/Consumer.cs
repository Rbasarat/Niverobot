using Microsoft.Extensions.Configuration;
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
        }
    }
}