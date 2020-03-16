using Domain;
using Microsoft.Extensions.Options;
using MihaZupan;
using Niverobot.WebApi.Interfaces;
using Telegram.Bot;

namespace Niverobot.WebApi.Services
{
    public class BotService : IBotService
    {
        private readonly BotConfiguration _botConfig;

        public BotService(IOptions<BotConfiguration> config)
        {
            _botConfig = config.Value;
            // use proxy if configured in appsettings.*.json
            Client = string.IsNullOrEmpty(_botConfig.Socks5Host)
                ? new TelegramBotClient(_botConfig.BotToken)
                : new TelegramBotClient(
                    _botConfig.BotToken,
                    new HttpToSocks5Proxy(_botConfig.Socks5Host, _botConfig.Socks5Port));
        }

        public TelegramBotClient Client { get; }

    }
}
