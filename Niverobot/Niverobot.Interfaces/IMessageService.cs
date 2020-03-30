using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Niverobot.Interfaces
{
    public interface IMessageService
    {
        Task HandleTextMessageAsync(Update update);
    }
}
