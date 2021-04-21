using System.Collections.Generic;
using System.Linq;
using KrugerNationalPark.Agents;
using Mars.Common.Core.Collections;
using Mars.Components.Environments;
using Mars.Components.Layers;
using Mars.Core.Data;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Layers;
using Mars.Interfaces.Model;
using Mars.Interfaces.Model.Import;
using Mars.Interfaces.Model.Options;

namespace KrugerNationalPark.Layers
{
    /// <summary>
    ///     This class represents the agent layer implementation for the <see cref="KnpCar" /> and keeps references
    ///     to all other required layer e.g., the <see cref="ElephantLayer" />
    /// </summary>
    public class TouristLayer : AbstractLayer
    {
        public TouristLayer(ElephantLayer elephantLayer)
        {
            ElephantLayer = elephantLayer;
        }
        
        public override bool InitLayer(LayerInitData layerInitData, RegisterAgent registerAgentHandle,
            UnregisterAgent unregisterAgentHandle)
        {
            base.InitLayer(layerInitData, registerAgentHandle, unregisterAgentHandle);

            StreetEnvironment = new SpatialGraphEnvironment(new SpatialGraphOptions
            {
                GraphImports = new List<Source>
                {
                    new()
                    {
                        File = layerInitData.LayerInitConfig.File,
                        InputConfiguration = new InputConfiguration
                        {
                            IsBiDirectedImport = true,
                            Modalities = new HashSet<SpatialModalityType> {SpatialModalityType.CarDriving}
                        }
                    }
                },
                NodeIndex = true
            });
            
            Tourists = Container.Resolve<IAgentManager>().Spawn<Tourist, TouristLayer>().ToList();
            
            return true;
        }

        public List<Tourist> Tourists { get; set; }

        [PropertyDescription]
        public ElephantLayer ElephantLayer { get; }
        
        public ISpatialGraphEnvironment StreetEnvironment { get; set; }
        
    }
}