using System;
using KrugerNationalPark.Layers;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Environments;
using SOHCarModel.Model;
using SOHCarModel.Steering;
using SOHDomain.Output;
using SOHDomain.Steering.Common;

namespace KrugerNationalPark.Agents
{
    public class Tourist : IAgent<TouristLayer>, ICarSteeringCapable, ITripSavingAgent
    {
        public void Init(TouristLayer layer)
        {
            TripsCollection = new TripsCollection(layer.Context);
            
            var car = layer.EntityManager.Create<KnpCar>("type", "Golf");
            car.Environment = layer.StreetEnvironment;
            car.TouristLayer = layer;
            
            Car = car;
            Position = Position.CreatePosition(StartLongitude, StartLatitude);

            Car.Mass = MyMass;
            
            car.TryEnterDriver(this, out var handle);

            var start = layer.StreetEnvironment.NearestNode(Position);
            layer.StreetEnvironment.Insert(car, start);
            var goal = layer.StreetEnvironment.GetRandomNode();
            handle.Route = layer.StreetEnvironment.FindRoute(start, goal);

            VehicleHandle = handle;
        }

        [PropertyDescription(Name = "myMass")]
        public double MyMass { get; set; }

        public CarSteeringHandle VehicleHandle { get; set; }

        public void Tick()
        {
            VehicleHandle.Move();
            TripsCollection.Add(Position);
        }
        
        public Guid ID { get; set; }
        
        [PropertyDescription(Name = "startLon")]
        public double StartLongitude { get; set; }
        
        [PropertyDescription(Name = "startLat")]
        public double StartLatitude { get; set; }

        public Position Position
        {
            get => Car.Position;
            set => Car.Position = value;
        }
        
        public void Notify(PassengerMessage passengerMessage)
        {
            
        }

        public bool OvertakingActivated { get; }
        public Car Car { get; set; }
        public bool CurrentlyCarDriving => true;
        public int StableId { get; }
        public TripsCollection TripsCollection { get; set; }
    }
}