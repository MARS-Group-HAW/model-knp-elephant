using System.Linq;
using Mars.Components.Layers;
using Mars.Interfaces.Environments;

namespace KrugerNationalPark.Layers
{
    public class VectorWaterLayer : VectorLayer 
    {
        public Position ExploreClosestFullPotentialField(double lat, double lon, double maxDistance)
        {
            return Explore(new double[]{lon, lat}, maxDistance, 1).FirstOrDefault().Node?.NodePosition;
        }
    }
}