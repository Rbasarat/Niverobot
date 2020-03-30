using Telegram.Bot;

namespace Niverobot.Interfaces
{
    public interface ITelegramBotService
    {
        TelegramBotClient Client { get; }
    }
}