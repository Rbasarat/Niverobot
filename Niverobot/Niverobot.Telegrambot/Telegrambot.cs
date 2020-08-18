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
        private readonly IMessageService _messageService;
        private readonly ITelegramBotService _telegramBotService;

        public Telegrambot(IMessageService messageService, ITelegramBotService telegramBotService)
        {
            _messageService = messageService;
            _telegramBotService = telegramBotService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var me = _telegramBotService.Client.GetMeAsync().Result;
            Log.Debug(
                $"Successfully connected to telegram."
            );
            _telegramBotService.Client.OnMessage += HandleMessage;
            _telegramBotService.Client.StartReceiving();
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

            _telegramBotService.Client.StopReceiving();
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