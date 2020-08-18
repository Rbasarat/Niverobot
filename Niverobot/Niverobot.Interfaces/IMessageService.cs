using System.Threading.Tasks;
using Telegram.Bot.Args;

namespace Niverobot.Interfaces
{
    public interface IMessageService
    {
        Task HandleTextMessageAsync(MessageEventArgs update);
    }
}
