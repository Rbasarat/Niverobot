using DateParser;

namespace Niverobot.Interfaces
{
    public interface IGRPCService
    {
        ParseDateReply ParseDateTimeFromNl(string date);
    }
}