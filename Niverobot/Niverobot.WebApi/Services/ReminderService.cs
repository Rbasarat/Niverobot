using Microsoft.Extensions.Configuration;
using Niverobot.Domain.EfModels;
using Niverobot.WebApi.Interfaces;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Niverobot.WebApi.Services
{
    public class ReminderService : IReminderService
    {
        private string MessagePattern = @"([""'])(?:(?=(\\?))\2.)*?\1";
        private string DurationPattern = @"\d+|\D+";
        private RegexOptions options = RegexOptions.Multiline;
        private readonly ITelegramBotService _telegramBotService;
        private readonly IConfiguration Configuration;

        public ReminderService(ITelegramBotService telegramBotService, IConfiguration configuration)
        {
            _telegramBotService = telegramBotService;
            Configuration = configuration;
        }
        public async Task SetReminderAsync(Update update)
        {
            // TODO: check for format of string.
            try
            {

                using (var db = new NiveroBotContext(Configuration))
                {
                    var reminder = new Reminder
                    {
                        SenderId = update.Message.From.Id,
                        ReceiverId = update.Message.Chat.Id,
                        SenderUserName = update.Message.From.Username
                    };
                    reminder.Message = Regex.Matches(update.Message.Text, MessagePattern, options)[0].Value;

                    // Get the single last argument
                    var commands = update.Message.Text.Split(' ');
                    var duration = commands[commands.Count() - 1];
                    var timezone = commands.Last();
                    reminder.Date = ConvertDurationToDateTime(duration).AddHours(Double.Parse(timezone));
                    Configuration.GetConnectionString("SqlServer");
                    db.Reminders.Add(reminder);
                    db.SaveChanges();
                }

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
            }
        }

        private DateTime ConvertDurationToDateTime(string duration)
        {
            var matches = Regex.Matches(duration, DurationPattern, options);
            switch (matches[1].Value)
            {
                case "s":
                    return DateTime.Now.AddSeconds(Double.Parse(matches[0].Value));
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
    }
}
