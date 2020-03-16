using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Niverobot.WebApi.Interfaces
{
    public interface ITelegramUpdateService
    {
        Task EchoAsync(Update update);
    }
}
