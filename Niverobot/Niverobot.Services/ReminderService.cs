﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Niverobot.Domain.EfModels;
using Niverobot.Interfaces;
using Serilog;
using Telegram.Bot.Types;

namespace Niverobot.Services
{
    public class ReminderService : IReminderService
    {
        private readonly ITelegramBotService _telegramBotService;
        private readonly IGRPCService _grpcService;
        private readonly NiveroBotContext _context;

        public ReminderService(ITelegramBotService telegramBotService, IGRPCService grpcService,
            NiveroBotContext context)
        {
            _telegramBotService = telegramBotService;
            _grpcService = grpcService;
            _context = context;
        }

        public async Task HandleReminderAsync(Update update)
        {
            // .reminders to show list of all reminders
            // You can then edit your own reminders and delete them.
            if (update.Message.Text.Contains("-h"))
            {
                await SendHelpMessage(update.Message.Chat.Id);
            }
            else
            {
                await SaveReminderAsync(update);
            }
        }

        private async Task SendHelpMessage(long chatId)
        {
            // TODO: Reminder for someone else
            // TODO: Repeatable reminders
            // TODO: .reminders to show list of all reminders
            // TODO: You can then edit your own reminders and delete them.
            //TODO: maybe fix tomorrow?

            await _telegramBotService.Client.SendTextMessageAsync(
                chatId: chatId,
                text: "Use .reminder to set a reminder for yourself or for a channel.\n" +
                      "Some examples include:\n" +
                      "`.reminder drink water at 3pm`\n" +
                      "`.reminder wish Linda happy birthday on June 1st`\n" +
                      "`.reminder \"Update the project status\" on Monday at 9am`\n" +
                      "`.reminder reminder! interview in 3 hours`\n",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
            );
            ;
        }

        private async Task SaveReminderAsync(Update update)
        {
            try
            {
                var response = _grpcService.ParseDateTimeFromNl(update.Message.Text);

                if (response.ParsedDate.Seconds <= 1)
                {
                    // TODO centralize error handling
                    await _telegramBotService.Client.SendTextMessageAsync(
                        chatId: update.Message.Chat.Id,
                        text: "Sorry i did not understand."
                    );
                    Log.Error("Error setting parsing date: {0}", update.Message);
                }
                else
                {
                    var parsedDateTime = response.ParsedDate.ToDateTime();
                    var utcDate = parsedDateTime.AddSeconds(response.Offset);
                    var reminder = new Reminder
                    {
                        SenderId = update.Message.From.Id,
                        ReceiverId = update.Message.Chat.Id,
                        SenderUserName = JsonConvert.SerializeObject(update),
                        // SenderUserName = update.Message.From.Username ?? update.Message.From.FirstName,
                        // Remove trigger word and time from message
                        Message = update.Message.Text.Replace(".reminder ", "").Replace(response.Date, ""),
                        TriggerDate = utcDate
                    };

                    _context.Reminders.Add(reminder);
                    _context.SaveChanges();

                    await _telegramBotService.Client.SendTextMessageAsync(
                        chatId: update.Message.Chat.Id,
                        text: "Reminder is set."
                    );
                }
            }
            catch (Exception e)
            {
                // TODO: centralize error handling
                await _telegramBotService.Client.SendTextMessageAsync(
                    chatId: update.Message.Chat.Id,
                    text: "Error setting reminder. Please try again."
                );
                Log.Error("Error setting reminder: {0}", e);
            }
        }

        public async Task SendReminderAsync(Reminder reminder)
        {
            var message = ("Reminder: {0}", reminder.Message);
            await _telegramBotService.Client.SendTextMessageAsync(
                chatId: reminder.SenderId,
                text: $"Reminder: {reminder.Message} \n\n Sender: {reminder.SenderUserName}",
                Telegram.Bot.Types.Enums.ParseMode.Markdown
            );
        }

        public IQueryable<Reminder> GetReminders(DateTime currentDate)
        {
            return _context.Reminders.Where(x => x.TriggerDate > currentDate).Take(10);
        }
    }
}