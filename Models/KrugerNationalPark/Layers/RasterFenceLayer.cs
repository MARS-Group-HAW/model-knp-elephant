using System;
using Mars.Components.Layers;
using Position = Mars.Interfaces.Environments.Position;

namespace KrugerNationalPark.Layers
{
    /// <summary>
    ///     This layer represents the fence of the kruger park.
    /// </summary>
    public class RasterFenceLayer : RasterLayer
    {
        /// <summary>
        ///     Checks for the coordinate whether this point is inside the fence.
        /// </summary>
        /// <param name="coordinate">The coordinate to check</param>
        /// <returns>
        ///    Returns true when the coordinate is inside the fence.
        /// </returns>
        public bool IsPointInside(Position coordinate)
        {
            return Extent.Contains(coordinate.X, coordinate.Y) && Math.Abs(GetValue(coordinate) - 1) < 0.001;
        }
    }
}