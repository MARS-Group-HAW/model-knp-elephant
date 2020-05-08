using System;
using Mars.Components.Layers;
using Position = Mars.Interfaces.Environment.Position;

namespace KrugerNationalPark.Layers
{
    public class GISRasterFenceLayer : GISRasterModelLayer
    {
        public bool IsPointInside(Position coord)
        {
            return base.Extent.Contains(coord.X, coord.Y) && Math.Abs(base.GetValue(coord) - 1) < 0.001;
        }
    }
}