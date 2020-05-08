using KrugerNationalPark.Agents;
using Mars.Interfaces.Layer;
using Mars.Interfaces.Layer.Initialization;
using SOHCarLayer.Model;

namespace KrugerNationalParkTraffic
{
    public class KnpCarLayer : CarLayer
    {
        public ElephantLayer ElephantLayer { get; }

        public KnpCarLayer(ElephantLayer elephantLayer)
        {
            ElephantLayer = elephantLayer;
        }
    }
}