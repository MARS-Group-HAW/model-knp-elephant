using System;
using Mars.Components.Environments;
using Mars.Components.Services;
using Mars.Interfaces.Agent;
using Mars.Interfaces.Environment;
using Mars.Interfaces.Layer;
using Mars.Interfaces.Layer.Initialization;

namespace KrugerNationalPark.Layers
{
    public class MyType : IEntity, IPositionable
    {
        public Guid ID { get; set; }

        public Position Position { get; set; }
        
    }
    public class GridLayer : ILayer
    {
        
        public SpatialHashEnvironment<MyType> Environment { get; set; }
        
        public bool InitLayer(TInitData layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle)
        {
            
             Environment = new SpatialHashEnvironment<MyType>(100, 100);
             return true;
        }

        public long GetCurrentTick()
        {
            throw new System.NotImplementedException();
        }

        public void SetCurrentTick(long currentStep)
        {
            throw new System.NotImplementedException();
        }
    }
}