using SOHCarLayer.Domain;
using SOHCarLayer.Model;

namespace KrugerNationalParkTraffic
{
    public class KnpCar : Car
    {
        public KnpCar(ICarLayer layer, double length, double maxAcceleration, double maxDeceleration, double maxSpeed,
            double velocity, int passengerCapacity, string trafficCode = "south-african", string stableId = "") : base(layer,
            length, maxAcceleration, maxDeceleration, maxSpeed, velocity, passengerCapacity, trafficCode, stableId)
        { }

        protected override CarSteeringHandle CreateSteeringHandle(ICarSteeringCapable steeringCapable)
        {
            return new KnpCarSteeringHandle(Environment, this, steeringCapable);
        }
    }
}