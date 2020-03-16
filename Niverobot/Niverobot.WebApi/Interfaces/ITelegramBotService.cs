using Telegram.Bot;

namespace Niverobot.WebApi.Interfaces
{
    public interface ITelegramBotService
    {
        TelegramBotClient Client { get; }
    }
}