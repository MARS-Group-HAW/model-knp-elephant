using System.Collections.Generic;
using System.Linq;
using Mars.Components.Environments;
using Mars.Interfaces.Environment.SpatialGraphEnvironment;
using Mars.Interfaces.Layer;
using SOHCarLayer.Domain;
using SOHCarLayer.Model;
using SOHCarLayer.Routing;
using SOHDomain.Output.Trips;

namespace KrugerNationalPark.Agents
{
    public class KnpCarDriver : CarDriver
    {
        private CarSteeringHandle _knpCarSteeringHandle;

        public KnpCarDriver(
            KnpCarLayer layer, RegisterAgent register, UnregisterAgent unregister,
            double length, double maxAcceleration, double maxDeceleration,
            double maxSpeed, int driveMode = 1, double startLat = 0, double startLon = 0,
            double destLat = 0, double destLon = 0, double velocity = 0,
            ISpatialEdge startingEdge = null, int capacity = 5, string stableId = "",
            string trafficCode = "south-african", string osmRoute = "")
            : base(layer, register, unregister, length, maxAcceleration, maxDeceleration,
                maxSpeed, driveMode, startLat, startLon, destLat, destLon, velocity,
                startingEdge, capacity, stableId, trafficCode, osmRoute)
        {
            Trip = new List<TripPosition>();
            Car = new KnpCar(layer,this, length, maxAcceleration, maxDeceleration, maxSpeed, velocity, capacity);
            
            var route = CarRouteFinder.Find(layer.Environment, driveMode, 
                startLat, startLon, destLat, destLon, startingEdge, osmRoute);
            var node = route.First().Edge.From;
            
            layer.Environment.Insert(Car, node);
            Car.Position = node.Position;
            Car.TryEnterDriver(this, out _knpCarSteeringHandle);
            _knpCarSteeringHandle.Route = route;
        }

        public List<TripPosition> Trip { get; }
        
        public double Bearing { get; set; }

        /// <summary>
        ///     Will be called by the MARS Framework when a new simulation tick shall be made.
        ///     Usually you shouldn't call this method in your own code. Instead use the RegisterAgentHandle of each layers'
        ///     Initialization Method to register your Agent for execution at the LayerContainer
        /// </summary>
        public override void Tick()
        {
            base.Tick();
            Position = Car.CalculateNewPositionFor(_knpCarSteeringHandle.Route, out var bearing);
            Bearing = bearing;
        }
        
    }
}