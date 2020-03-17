using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Niverobot.WebApi.Interfaces
{
    public interface IDadJokeService
    {
        Task<string> GetDadJokeAsync();
    }
}
