using Telegram.Bot;

namespace Niverobot.WebApi.Interfaces
{
    public interface IBotService
    {
        TelegramBotClient Client { get; }
    }
}