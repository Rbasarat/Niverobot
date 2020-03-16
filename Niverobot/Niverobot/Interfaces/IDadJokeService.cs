using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Niverobot.Interfaces
{
    interface IDadJokeService
    {
        Task<string> GetDadJokeAsync();
    }
}
