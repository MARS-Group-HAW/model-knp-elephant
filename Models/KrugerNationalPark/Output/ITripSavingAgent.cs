namespace KrugerNationalPark.Output
{
    /// <summary>
    ///     Has a <see cref="Output.TripsCollection" /> that stores all trips.
    /// </summary>
    public interface ITripSavingAgent
    {
        /// <summary>
        ///     Uniquely identifies this agent.
        /// </summary>
        int StableId { get; }

        /// <summary>
        ///     Contains all trip information.
        /// </summary>
        TripsCollection TripsCollection { get; }
    }
}