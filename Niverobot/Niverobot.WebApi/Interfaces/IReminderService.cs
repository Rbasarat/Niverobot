using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Niverobot.WebApi.Interfaces
{
    public interface IReminderService
    {
        Task SetReminderAsync(Update update);
    }
}
