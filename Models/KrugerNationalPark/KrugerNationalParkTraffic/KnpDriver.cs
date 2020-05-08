using System.Collections.Generic;
using Mars.Interfaces.Environment.SpatialGraphEnvironment;
using Mars.Interfaces.Layer;
using SOHCarLayer.Model;
using SOHDomain.Output.Trips;

namespace KrugerNationalParkTraffic
{
    public class KnpDriver : CarDriver
    {
        public KnpDriver(
            CarLayer layer, RegisterAgent register, UnregisterAgent unregister,
            double length, double maxAcceleration, double maxDeceleration,
            double maxSpeed, int driveMode, double startLat = 0, double startLon = 0,
            double destLat = 0, double destLon = 0, double velocity = 0,
            ISpatialEdge startingEdge = null, int capacity = 5, string stableId = "",
            string trafficCode = "german", string osmRoute = "")
            : base(layer, register, unregister, length, maxAcceleration, maxDeceleration,
                maxSpeed, driveMode, startLat, startLon, destLat, destLon, velocity,
                startingEdge, capacity, stableId, trafficCode, osmRoute)
        {
            
        }

        public List<TripPosition> Trip { get; set; }
    }
}