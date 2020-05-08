using System;
using System.Linq;
using Mars.Components.Layers;
using Mars.Interfaces.Environment;

namespace KrugerNationalPark.Layers
{
    public class KNPGISRasterShadeLayer : RasterLayer
    {
        private const int FullPotential = 100;

        /// <summary>
        ///     Searches for nearest full potential field.
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <param name="maxDistance"></param>
        /// <returns>
        ///    Returns the coordinates of the field or null if no shades were found or current field has 0 value.
        /// </returns>
        public Position ExploreClosestFullPotentialField(double lat, double lon, int maxDistance)
        {
            if (IsPointInside(Position.CreateGeoPosition(lon, lat)))
            {
                var res = Explore(Position.CreateGeoPosition(lon, lat), maxDistance).FirstOrDefault();

                if (res.Node?.NodePosition != null)
                {
                    var targetLon = LowerLeft.X + res.Node.NodePosition.X * base.CellWidth;
                    var targetLat = LowerLeft.Y + res.Node.NodePosition.Y * base.CellHeight;

                    return Position.CreateGeoPosition(targetLon, targetLat);
                }
            }

            return null;
        }

        /// <summary>
        ///     Returns true if there is max potential on the requested cell
        /// </summary>
        /// <param name="lat">Lat/y coordinate component</param>
        /// <param name="lon">Lon/x coordinate component</param>
        /// <returns>
        ///    Returns true when a full potential is at the desired cell. 
        /// </returns>
        public bool HasFullPotential(double lat, double lon)
        {
            if (Extent.Contains(lon, lat))
            {
                var value = GetValue(Position.CreateGeoPosition(lon, lat));
                return Math.Abs(FullPotential - value) < 0.00001;
            }

            return false;
        }
        
        private bool IsPointInside(Position coordinate)
        {
            return base.Extent.Contains(coordinate.X, coordinate.Y) && Math.Abs(base.GetValue(coordinate) - 1) < 0.001;
        }
    }
}