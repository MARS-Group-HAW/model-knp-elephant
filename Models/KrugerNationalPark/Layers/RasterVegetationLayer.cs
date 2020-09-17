using System;
using Mars.Components.Layers;
using Mars.Interfaces.Environments;

namespace KrugerNationalPark.Layers
{
    /// <summary>
    ///     This raster layer provides information about biomass of animals etc.
    /// </summary>
    public class RasterVegetationLayer : RasterLayer
    {
        public bool IsPointInside(Position coordinate)
        {
            return Extent.Contains(coordinate.X, coordinate.Y) && Math.Abs(GetValue(coordinate) - 1) < 0.001;
        }
    }
}