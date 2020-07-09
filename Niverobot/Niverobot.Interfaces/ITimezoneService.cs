namespace Niverobot.Interfaces
{
    public interface ITimezoneService
    {
        int GetUtcOffsetInSeconds(double lat, double lon);
    }
}