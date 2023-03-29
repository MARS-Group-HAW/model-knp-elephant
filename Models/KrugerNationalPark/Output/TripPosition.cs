using NetTopologySuite.Geometries;

namespace KrugerNationalPark.Output
{
    public class TripPosition : Coordinate
    {
        public TripPosition(double longitude, double latitude) : base(longitude, latitude)
        {
        }

        public int UnixTimestamp { get; set; }
    }
}