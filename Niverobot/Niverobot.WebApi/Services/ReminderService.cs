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

namespace Niverobot.WebApi.Services
{
    public class ReminderService : IReminderService
    {
        private string GetDatePattern = @"[a-zA-Z]";
        private string DurationPattern = @"\d+|\D+";
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
            var flag = update.Message.Text.Replace(".reminder", "");
            if (flag.Contains("-h"))
            {
                await _telegramBotService.Client.SendTextMessageAsync(
                    chatId: update.Message.Chat.Id,
                    text: "To set a reminder use the following command:\n\n" +
                    "*.reminder <time> <message>* \n\n" +
                    "*<time> formats*: 1m/1h/1d/1M/1y or hh:mm dd-MM-YYY\n" +
                    "Examples: \n" +
                    ".reminder 1M Lunch with Jake (1 month)\n" +
                    ".reminder 1d Lunch with Jake (1 day)\n" +
                    ".reminder 15:36 12-03-2021 Lunch with Jake\n",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
                ); ;
            }
            else
            {
                await SetReminderAsync(update);
            }
        }

        public async Task SetReminderAsync(Update update)
        {
            // TODO: check for format of string.
            try
            {

                var reminder = new Reminder
                {
                    SenderId = update.Message.From.Id,
                    ReceiverId = update.Message.Chat.Id,
                    SenderUserName = update.Message.From.Username
                };

                var command = update.Message.Text.Split(' ');
                // Remove triggerword.
                command = command.Skip(1).ToArray();

                reminder.Message = command.Last();

                string triggerDate = command.First();

                if (Regex.IsMatch(triggerDate, GetDatePattern))
                {
                    reminder.TriggerDate = ConvertDurationToDateTime(triggerDate);
                }
                else
                {
                    // TODO parse whole dates.
                }

                _context.Reminders.Add(reminder);
                _context.SaveChanges();


                await _telegramBotService.Client.SendTextMessageAsync(
                    chatId: update.Message.Chat.Id,
                    text: "Reminder is set."
                );

            }
            catch (Exception e)
            {
                await _telegramBotService.Client.SendTextMessageAsync(
                    chatId: update.Message.Chat.Id,
                    text: "Error setting reminder. Please try again."
                );
                Log.Error("Error setting reminder: {0}", e);
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

            return zoneLocations.Where(x => x.ZoneId.ToLower().Contains(tz) == true).FirstOrDefault();
        }
    }
}
