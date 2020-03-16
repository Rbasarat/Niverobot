using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Niverobot.WebApi.Interfaces
{
    public interface IUpdateService
    {
        Task EchoAsync(Update update);
    }
}
