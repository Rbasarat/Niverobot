using GeoTimeZone;
using Microsoft.Extensions.Configuration;
using Niverobot.Domain.EfModels;
using Niverobot.WebApi.Interfaces;
using NodaTime;
using NodaTime.TimeZones;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Serilog;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Niverobot.WebApi.Services
{
    public class ReminderService : IReminderService
    {
        private const string DurationPattern = @"\d+|\D+";

        private const string MessagePattern = @"(.*)\b(in|on|at)\b\s+(.*)";
        private RegexOptions options = RegexOptions.Multiline;
        private readonly ITelegramBotService _telegramBotService;
        private readonly NiveroBotContext _context;

        public ReminderService(ITelegramBotService telegramBotService, NiveroBotContext context)
        {
            _telegramBotService = telegramBotService;
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
                await SetReminderAsync(update);
            }
        }

        private async Task SendHelpMessage(long chatId)
        {
            // TODO: Reminder for someone else
            // TODO: Repeatable reminders
            // TODO: .reminders to show list of all reminders
            // TODO: You can then edit your own reminders and delete them.

            await _telegramBotService.Client.SendTextMessageAsync(
                chatId: chatId,
                text: "Use .reminder to set a reminder for yourself or for a channel.\n" +
                      "Use the following format:\n *.reminder <message> on/in/at <time>*\n" +
                      "Some examples include:\n" +
                      "`.reminder drink water at 3pm`\n" +
                      "`.reminder wish Linda happy birthday on June 1st`\n" +
                      "`.reminder \"Update the project status\" on Monday at 9am`\n" +
                      "`.reminder reminder! interview in 3 hours`\n",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
            );
            ;
        }

        private async Task SetReminderAsync(Update update)
        {
            // TODO: check for format of string.
            // Remove triggerword from message
            var message = update.Message.Text.Replace(".reminder ", "");
            var groups = Regex.Matches(message, MessagePattern).FirstOrDefault()?.Groups;
            
            if (groups == null || groups.Count != 4)
            {
                // TODO: centralize error handling
                await _telegramBotService.Client.SendTextMessageAsync(
                    chatId: update.Message.Chat.Id,
                    text: "Reminder format is not valid,\n" +
                          "Use `.reminder -h` for help.",
                    ParseMode.Markdown
                );
            }
            else
            {
                // Remove firsts group.
                var matches = groups.Values.Skip(1);
                try
                {
                    var reminder = new Reminder
                    {
                        SenderId = update.Message.From.Id,
                        ReceiverId = update.Message.Chat.Id,
                        SenderUserName = update.Message.From.Username,
                        Message = matches.First().Value
                    };
                    // TODO Call dateTime module and set triggerdate.
                    
                    // _context.Reminders.Add(reminder);
                    // _context.SaveChanges();

                    await _telegramBotService.Client.SendTextMessageAsync(
                        chatId: update.Message.Chat.Id,
                        text: "Reminder is set."
                    );
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


        }

        private DateTime ConvertDurationToDateTime(string duration)
        {
            var matches = Regex.Matches(duration, DurationPattern, options);
            switch (matches[1].Value)
            {
                case "m":
                    return DateTime.Now.AddMinutes(Double.Parse(matches[0].Value));
                case "h":
                    return DateTime.Now.AddHours(Double.Parse(matches[0].Value));
                case "d":
                    return DateTime.Now.AddDays(Double.Parse(matches[0].Value));
                case "M":
                    return DateTime.Now.AddMonths(int.Parse(matches[0].Value));
                case "y":
                    return DateTime.Now.AddYears(int.Parse(matches[0].Value));
                default:
                    // TODO throw error.
                    return DateTime.Now;
            }
        }

        private TzdbZoneLocation GetTimeZoneFromLatLong(double latitude, double longitude)
        {
            var zoneLocations = TzdbDateTimeZoneSource.Default.ZoneLocations;
            if (zoneLocations == null) return null;

            var tz = TimeZoneLookup.GetTimeZone(latitude, longitude).Result.ToLower();

            return zoneLocations.FirstOrDefault(x => x.ZoneId.ToLower().Contains(tz) == true);
        }
    }
}