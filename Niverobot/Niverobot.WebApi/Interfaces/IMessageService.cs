using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Niverobot.WebApi.Interfaces
{
    public interface IMessageService
    {
        Task HandleTextMessageAsync(Update update);
    }
}
