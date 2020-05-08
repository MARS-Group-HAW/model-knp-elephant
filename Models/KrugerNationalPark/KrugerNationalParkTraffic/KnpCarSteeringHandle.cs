using System;
using Mars.Interfaces.Environment.SpatialGraphEnvironment;
using SOHCarLayer.Domain;
using SOHCarLayer.Model;

namespace KrugerNationalParkTraffic
{
    public class KnpCarSteeringHandle : CarSteeringHandle
    {
        public KnpCarSteeringHandle(ISpatialGraphEnvironment environment, Car car, ICarSteeringCapable steeringCapable) 
            : base(environment, car, steeringCapable)
        {
            
        }

        /// <summary>Provides the possibility to tick the moving road user.</summary>
        public override void Move()
        {
            base.Move();
        }
        
        private void HandleWildlifeAhead(double speedElephantAhead, double distanceElephantAhead)
        {
            // Calculate the full stop speed change when wildlife was detected
            var speedChange = VehicleAccelerator.CalculateSpeedChange(this._car.Velocity, SpeedLimit, 
                distanceElephantAhead, speedElephantAhead);
            // Is used when the movement is performed
            if (speedChange < BiggestDeceleration)
                BiggestDeceleration = speedChange;
        }

        private bool IsWildlifeAhead(out double speedElephant, out double distanceElephant)
        {
            // @Thomas: Use this to define your condition when the wildlife is ahead
            var elephantLayer = Layer.ElephantLayer;
            var enumerable = elephantLayer.Environment.Explore(_car.Position, 300, 1);
            
            //TODO Check for wildlife in the area by exploring elephants + rule set about how to react

            // Did we explore any elephant within 100 meter then wildlife detected
            // maybe the exploration should be within a cone of the car

            distanceElephant = -999;
            speedElephant = -999;
            
            if (enumerable.Any())
            {
                speedElephant = 5;
                distanceElephant = enumerable.First().Position.DistanceInKmTo(thi_car.Position) * 1000;
                //Console.WriteLine("Elephant ahead in: " + distanceElephant + " m");
                return true;
            }

            return false;
        }
    } 
}