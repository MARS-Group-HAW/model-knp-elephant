using System.Collections.Generic;
using SOHCarLayer.Domain;
using SOHCarLayer.Model;
using SOHDomain.Output.Trips;

namespace KrugerNationalPark.Agents
{
    /// <summary>
    ///     This class implements custom properties and the binding to the KNP related <see cref="KnpCarSteeringHandle"/>.
    ///     Those logic is used for this custom car. Use this class in order to expect more parameters.
    /// </summary>
    public class KnpCar : Car
    {
        
        private readonly KnpCarLayer _layer;

        public KnpCar(KnpCarLayer layer, KnpCarDriver carDriver, double length, double maxAcceleration, double maxDeceleration, double maxSpeed,
            double velocity, int passengerCapacity, string trafficCode = "south-african", string stableId = "") : base(layer,
            length, maxAcceleration, maxDeceleration, maxSpeed, velocity, passengerCapacity, trafficCode, stableId)
        {
            CarDriver = carDriver;
            _layer = layer;
        }

        public KnpCarDriver CarDriver { get; }

        protected override CarSteeringHandle CreateSteeringHandle(ICarSteeringCapable steeringCapable)
        {
            return new KnpCarSteeringHandle(_layer, Environment, this, steeringCapable);
        }
    }
}