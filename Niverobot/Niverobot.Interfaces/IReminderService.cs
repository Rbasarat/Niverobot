using System;
using System.Linq;
using System.Threading.Tasks;
using Niverobot.Domain.EfModels;
using Telegram.Bot.Types;

namespace Niverobot.Interfaces
{
    public interface IReminderService
    {
        Task HandleReminderAsync(Update update);
        Task SendReminderAsync(Reminder reminder);
        IQueryable<Reminder> GetNotSendReminders(DateTime currentDate);
        void SetReminderSend(int id);
    }
}
