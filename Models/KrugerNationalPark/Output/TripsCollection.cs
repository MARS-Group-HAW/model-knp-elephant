using System;
using System.Collections.Generic;
using System.Linq;
using Mars.Interfaces;
using Mars.Interfaces.Environments;

namespace KrugerNationalPark.Output
{
    /// <summary>
    ///     Holds <see cref="TripPosition" />s saved in a temporal order and structured by any object type.
    /// </summary>
    public sealed class TripsCollection
    {
        private static readonly DateTime ReferenceDateTime = new(1970, 1, 1);
        private readonly ISimulationContext _context;

        public TripsCollection(ISimulationContext context)
        {
            _context = context;
        }

        /// <summary>
        ///     Holds all <see cref="TripPosition" />s structured by an object.
        /// </summary>
        public List<(object, List<TripPosition>)> Result { get; } = new();

        /// <summary>
        ///     Saves the given position in the collection using the last used key to structure it..
        /// </summary>
        /// <param name="position">That is stored.</param>
        public void Add(Position position)
        {
            Add(Result.Any() ? Result.Last().Item1 : null, position);
        }

        /// <summary>
        ///     Saves the given position for given key in the collection. If the last key was equals to given key, the
        ///     position is added to existing (sub-)collection. Otherwise create a new subcollection with given key and add.
        /// </summary>
        /// <param name="key">Used to collate positions that belong together.</param>
        /// <param name="position">That is stored.</param>
        public void Add(object key, Position position)
        {
            var clock = _context.CurrentTimePoint.GetValueOrDefault();
            var time = (int) clock.Subtract(ReferenceDateTime).TotalSeconds;
            var tripPosition = new TripPosition(position.Longitude, position.Latitude) {UnixTimestamp = time};
            key ??= 0x01;
            var (previousKey, tripPositions) = Result.LastOrDefault();
            if (previousKey != null && previousKey.Equals(key))
                tripPositions.Add(tripPosition);
            else
                Result.Add((key, new List<TripPosition> {tripPosition}));
        }
    }
}