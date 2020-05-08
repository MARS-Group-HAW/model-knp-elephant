using System;
using Mars.Components.Layers;
using Mars.Interfaces.Environment;

namespace KrugerNationalPark.Layers
{
    public class KNPGISRasterVegetationLayer : RasterLayer
    {
        public bool IsPointInside(Position coord)
        {
            return base.Extent.Contains(coord.X, coord.Y) && Math.Abs(base.GetValue(coord) - 1) < 0.001;
        }
    }
}