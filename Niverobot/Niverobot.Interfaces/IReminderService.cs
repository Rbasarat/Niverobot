using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Niverobot.Interfaces
{
    public interface IReminderService
    {
        Task HandleReminderAsync(Update update);
    }
}
