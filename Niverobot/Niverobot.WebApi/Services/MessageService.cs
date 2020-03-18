using Niverobot.WebApi.Interfaces;
using Serilog;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Niverobot.WebApi.Services
{
    public class MessageService : IMessageService
    {
        private readonly ITelegramBotService _telegramBotService;
        private readonly IDadJokeService _dadJokeService;

        public MessageService(ITelegramBotService telegramBotService, IDadJokeService dadJokeService)
        {
            _telegramBotService = telegramBotService;
            _dadJokeService = dadJokeService;
        }

        public async Task HandleTextMessageAsync(Update update)
        {
            var message = update.Message;

            switch (update.Message.Text.Split(' ').First())
            {
                case ".dadjoke":
                    Log.Information("received dadjoke with chat id:{0}", message.Chat.Id);
                    var joke = await _dadJokeService.GetDadJokeAsync();
                    await _telegramBotService.Client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: joke
                    );
                    break;
                case ".niverhelp":

                    const string usage = "Usage:\n" +
                        ".dadjoke   - Tells you a dadjoke\n";

                    await _telegramBotService.Client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: usage,
                        replyMarkup: new ReplyKeyboardRemove()
                    );
                    break;
            }
        }
    }
}
