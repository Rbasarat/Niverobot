using System.Threading.Tasks;

namespace Niverobot.Interfaces
{
    public interface IDadJokeService
    {
        Task<string> GetDadJokeAsync();
    }
}
