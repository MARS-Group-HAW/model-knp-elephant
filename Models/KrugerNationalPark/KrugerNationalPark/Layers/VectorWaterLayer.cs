using Mars.Components.Layers;
using Mars.Interfaces.Environment;

namespace KrugerNationalPark.Layers
{
    public class VectorWaterLayer : VectorLayer 
    {
        public Position ExploreClosestFullPotentialField(double lat, double lon, double maxDistance)
        {
            return GetClosestPoint(Position.CreateGeoPosition(lon,lat), maxDistance);
        }
    }
}