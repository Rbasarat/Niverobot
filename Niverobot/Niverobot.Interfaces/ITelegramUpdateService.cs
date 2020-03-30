using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Niverobot.Interfaces
{
    public interface ITelegramUpdateService
    {
        Task EchoAsync(Update update);
    }
}
