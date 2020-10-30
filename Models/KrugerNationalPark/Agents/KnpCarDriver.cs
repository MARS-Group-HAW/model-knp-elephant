using System.Collections.Generic;
using Mars.Interfaces.Environments.SpatialGraphEnvironment;
using Mars.Interfaces.Layers;
using SOHCarModel.Model;
using SOHDomain.Output;
using SOHMultimodalModel.Output.Trips;

namespace KrugerNationalPark.Agents
{
    public class KnpCarDriver : CarDriver
    {
        public KnpCarDriver(
            KnpCarLayer layer, RegisterAgent register, UnregisterAgent unregister,
            double length, double maxAcceleration, double maxDeceleration,
            double maxSpeed, int driveMode = 1, double startLat = 0, double startLon = 0,
            double destLat = 0, double destLon = 0, double velocity = 0,
            ISpatialEdge startingEdge = null, int capacity = 5, string stableId = "",
            string trafficCode = "south-african", string osmRoute = "")
            : base(layer, register, unregister, driveMode, startLat, startLon, destLat, destLon, startingEdge, osmRoute)
        {
            Trip = new List<TripPosition>();
        }

        public List<TripPosition> Trip { get; }

        protected override Car CreateCar()
        {
            return Layer.EntityManager.Create<KnpCar>("type", "Golf");
        }
    }
}