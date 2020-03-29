using Dateparser;

namespace Niverobot.WebApi.Interfaces
{
    public interface IGRPCService
    {
        ParseDateReply ParseDateTimeFromNl(string date);
    }
}