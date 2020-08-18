using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Niverobot.Interfaces;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;

namespace Niverobot.Telegrambot
{
    public class Telegrambot : IHostedService
    {
        static ITelegramBotClient _botClient;
        private readonly IConfiguration _config;
        private readonly IMessageService _messageService;

        public Telegrambot(IConfiguration config, IMessageService messageService)
        {
            _config = config;
            _messageService = messageService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _botClient = new TelegramBotClient(_config["BotConfiguration:BotToken"]);
            //
            var me = _botClient.GetMeAsync().Result;
            Log.Debug(
                $"Successfully connected to telegram."
            );
            _botClient.OnMessage += HandleMessage;
            _botClient.StartReceiving();
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

            _botClient.StopReceiving();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Log.Information("Telegrambot Hosted Service is stopping.");

            return Task.CompletedTask;
        }

        async void HandleMessage(object sender, MessageEventArgs e)
        {
            switch (e.Message.Type)
            {
                case MessageType.Text:
                    await _messageService.HandleTextMessageAsync(e);
                    break;
            }
        }
    }
}