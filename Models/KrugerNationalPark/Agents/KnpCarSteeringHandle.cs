using System;
using System.Linq;
using Mars.Interfaces.Environments;
using Mars.Mathematics;
using SOHCarModel.Steering;
using SOHDomain.Output;
using SOHDomain.Steering.Handles;

namespace KrugerNationalPark.Agents
{
    public class KnpCarSteeringHandle : CarSteeringHandle
    {
        private static readonly DateTime ReferenceDateTime = new DateTime(1970, 1, 1);
        private readonly KnpCarLayer _carLayer;

        public KnpCarSteeringHandle(KnpCarLayer carLayer,
            ISpatialGraphEnvironment environment, KnpCar car,
            ICarSteeringCapable steeringCapable)
            : base(environment, car)
        {
            _carLayer = carLayer;
            KnpCar = car;
        }

        public KnpCar KnpCar { get; }

        /// <summary>
        ///     Provides an entry point for specialized types to provide some extra logic into
        ///     the movement operation before the
        ///     <see
        ///         cref="VehicleSteeringHandle{TSteeringCapable,TPassengerCapable,TSteeringHandle,TPassengerHandle}.CalculateDrivingDistance" />
        ///     is called.
        /// </summary>
        protected override double HandleCustom(SpatialGraphExploreResult exploreResult, double deceleration)
        {
            if (IsWildlifeAhead(out var speedElephant, out var distanceElephant))
                return HandleWildlifeAhead(deceleration, speedElephant, distanceElephant);

            return deceleration;
        }

        /// <summary>Provides the possibility to tick the moving road user.</summary>
        public override void Move()
        {
            base.Move();
            SaveTripPosition();
        }

        private double HandleWildlifeAhead(double deceleration, double speedElephantAhead, double distanceElephantAhead)
        {
            // Calculate the full stop speed change when wildlife was detected
            var speedChange = VehicleAccelerator.CalculateSpeedChange(Vehicle.Velocity, SpeedLimit,
                distanceElephantAhead, speedElephantAhead);

            // Is used when the movement is performed
            return speedChange < deceleration ? speedChange : deceleration;
        }

        private bool IsWildlifeAhead(out double speedElephant, out double distanceElephant)
        {
            // @Thomas: Use this to define your condition when the wildlife is ahead
            var elephantLayer = _carLayer.ElephantLayer;
            var enumerable = elephantLayer.Environment.Explore(Vehicle.Position, 300, 1);

            //TODO Check for wildlife in the area by exploring elephants + rule set about how to react

            // Did we explore any elephant within 100 meter then wildlife detected
            // maybe the exploration should be within a cone of the car

            distanceElephant = -999;
            speedElephant = -999;


            var elephant = enumerable.FirstOrDefault();
            if (elephant != null)
            {
                speedElephant = 5;
                distanceElephant = Distance.Haversine(elephant.Position.PositionArray, Vehicle.Position.PositionArray);

                //Console.WriteLine("Elephant ahead in: " + distanceElephant + " m");
                return true;
            }

            return false;
        }

        private void SaveTripPosition()
        {
            var clock = _carLayer.Context.CurrentTimePoint.GetValueOrDefault();
            var time = (int) clock.Subtract(ReferenceDateTime).TotalSeconds;
            KnpCar.CarDriver.Trip.Add(new TripPosition(Position.Longitude, Position.Latitude) {UnixTimestamp = time});
        }
    }
}