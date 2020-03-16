﻿using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using Serilog;
using Niverobot.Interfaces;

namespace Niverobot
{
    class ConsoleApplication
    {
        private static TelegramBotClient Bot;

        private readonly IConfiguration _config;
        private readonly IDadJokeService _dadJokeService;
        public ConsoleApplication(IConfiguration config, IDadJokeService dadJokeService)
        {
            _config = config;
            _dadJokeService = dadJokeService;
        }

        public async Task RunAsync()
        {
            Bot = new TelegramBotClient(_config.GetSection("BotConfiguration:BotToken").Value);

            var me = await Bot.GetMeAsync();
            Console.Title = me.Username;

            Bot.OnMessage += BotOnMessageReceived;
            //Bot.OnMessageEdited += BotOnMessageReceived;
            //Bot.OnCallbackQuery += BotOnCallbackQueryReceived;
            //Bot.OnInlineQuery += BotOnInlineQueryReceived;
            //Bot.OnInlineResultChosen += BotOnChosenInlineResultReceived;
            //Bot.OnReceiveError += BotOnReceiveError;

            Bot.StartReceiving(Array.Empty<UpdateType>());
            Log.Information($"Start listening for @{me.Username}");

            Console.ReadLine();
            Bot.StopReceiving();
        }

        private async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;
            if (message == null || message.Type != MessageType.Text) return;

            switch (message.Text.Split(' ').First())
            {
                case ".dadjoke":
                    var joke = await _dadJokeService.GetDadJokeAsync();
                    await Bot.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: joke
                    );
                    break;
                // send inline keyboard
                //case "/inline":
                //    await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

                //    // simulate longer running task
                //    await Task.Delay(500);

                //    var inlineKeyboard = new InlineKeyboardMarkup(new[]
                //    {
                //        // first row
                //        new []
                //        {
                //            InlineKeyboardButton.WithCallbackData("1.1", "11"),
                //            InlineKeyboardButton.WithCallbackData("1.2", "12"),
                //        },
                //        // second row
                //        new []
                //        {
                //            InlineKeyboardButton.WithCallbackData("2.1", "21"),
                //            InlineKeyboardButton.WithCallbackData("2.2", "22"),
                //        }
                //    });
                //    await Bot.SendTextMessageAsync(
                //        chatId: message.Chat.Id,
                //        text: "Choose",
                //        replyMarkup: inlineKeyboard
                //    );
                //    break;

                //// send custom keyboard
                //case "/keyboard":
                //    ReplyKeyboardMarkup ReplyKeyboard = new[]
                //    {
                //        new[] { "1.1", "1.2" },
                //        new[] { "2.1", "2.2" },
                //    };
                //    await Bot.SendTextMessageAsync(
                //        chatId: message.Chat.Id,
                //        text: "Choose",
                //        replyMarkup: ReplyKeyboard
                //    );
                //    break;

                //// send a photo
                //case "/photo":
                //    await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.UploadPhoto);

                //    const string file = @"Files/tux.png";
                //    using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                //    {
                //        var fileName = file.Split(Path.DirectorySeparatorChar).Last();
                //        await Bot.SendPhotoAsync(
                //            chatId: message.Chat.Id,
                //            photo: new InputOnlineFile(fileStream, fileName),
                //            caption: "Nice Picture"
                //        );
                //    }
                //    break;

                //// request location or contact
                //case "/request":
                //    var RequestReplyKeyboard = new ReplyKeyboardMarkup(new[]
                //    {
                //        KeyboardButton.WithRequestLocation("Location"),
                //        KeyboardButton.WithRequestContact("Contact"),
                //    });
                //    await Bot.SendTextMessageAsync(
                //        chatId: message.Chat.Id,
                //        text: "Who or Where are you?",
                //        replyMarkup: RequestReplyKeyboard
                //    );
                //    break;

                case "/niverhelp":
                    const string usage = "Usage:\n" +
                        ".dadjoke   - Tells you a dadjoke\n";
                    await Bot.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: usage,
                        replyMarkup: new ReplyKeyboardRemove()
                    );
                    break;
            }
        }

        private async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
        {
            var callbackQuery = callbackQueryEventArgs.CallbackQuery;

            await Bot.AnswerCallbackQueryAsync(
                callbackQueryId: callbackQuery.Id,
                text: $"Received {callbackQuery.Data}"
            );

            await Bot.SendTextMessageAsync(
                chatId: callbackQuery.Message.Chat.Id,
                text: $"Received {callbackQuery.Data}"
            );
        }

        private async void BotOnInlineQueryReceived(object sender, InlineQueryEventArgs inlineQueryEventArgs)
        {
            Console.WriteLine($"Received inline query from: {inlineQueryEventArgs.InlineQuery.From.Id}");

            InlineQueryResultBase[] results = {
                // displayed result
                new InlineQueryResultArticle(
                    id: "3",
                    title: "TgBots",
                    inputMessageContent: new InputTextMessageContent(
                        "hello"
                    )
                )
            };
            await Bot.AnswerInlineQueryAsync(
                inlineQueryId: inlineQueryEventArgs.InlineQuery.Id,
                results: results,
                isPersonal: true,
                cacheTime: 0
            );
        }

        private void BotOnChosenInlineResultReceived(object sender, ChosenInlineResultEventArgs chosenInlineResultEventArgs)
        {
            Console.WriteLine($"Received inline result: {chosenInlineResultEventArgs.ChosenInlineResult.ResultId}");
        }

        private void BotOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
        {
            Log.Error("Received error: {0} — {1}",
                 receiveErrorEventArgs.ApiRequestException.ErrorCode,
                 receiveErrorEventArgs.ApiRequestException.Message
             );
        }

    }
}
