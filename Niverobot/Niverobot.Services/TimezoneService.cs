using System;
using System.Linq;
using Niverobot.Interfaces;
using NodaTime;
using NodaTime.TimeZones;

namespace Niverobot.Services
{
    public class TimezoneService : ITimezoneService
    {
        public int GetUtcOffsetInSeconds(double lat, double lon)
        {
            var timeZone = GetTimeZoneFromLatLong(lat, lon);
            DateTimeZone zone = DateTimeZoneProviders.Tzdb[timeZone.ZoneId];
            return DateTimeZoneProviders.Tzdb[timeZone.ZoneId].MaxOffset.Seconds;
        }
        
        private TzdbZoneLocation GetTimeZoneFromLatLong(double latitude, double longitude)
        {
            var zoneLocations = TzdbDateTimeZoneSource.Default.ZoneLocations;
            if (zoneLocations == null) return null;
            return zoneLocations.Aggregate((x,y) => (Math.Abs(x.Latitude - latitude) + Math.Abs(x.Longitude - longitude)) <(Math.Abs(y.Latitude - latitude) + Math.Abs(y.Longitude - longitude)) ? x : y);
        }
        
    }
}