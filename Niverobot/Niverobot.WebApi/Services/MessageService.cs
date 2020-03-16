using Niverobot.WebApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Niverobot.WebApi.Services
{
    public class MessageService : IMessageService
    {
        public string HandleTextMessage()
        {
            return "test";
        }
    }
}
