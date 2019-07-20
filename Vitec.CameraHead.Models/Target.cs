namespace Vitec.CameraHead.Models
{
    using System.Linq;

    /// <summary>
    /// Immutable class representing a position within a studio or conference hall, this may be a weather map, a clock or a seat.
    /// </summary>
    /// <remarks>
    /// A target can have stored one or more shots, with each shot representing a single camera head position (pan/tilt), used to point to a target.
    /// </remarks>
    public sealed class Target
    {
        /// <summary>
        /// Gets the unique name for the target.
        /// </summary>
        /// <remarks>
        /// The name of the target is only guaranteed to be unique within the studio.
        /// </remarks>
        public string Name { get; }

        /// <summary>
        /// Gets all of the shots stored for a target.
        /// </summary>
        public Shot[] Shots { get; }

        /// <summary>
        /// Initialises a new instance of the <see cref="Target"/> class.
        /// </summary>
        /// <param name="name">Unique name of the target</param>
        /// <param name="shots">Shots pointing at the target (1 per head/camera).</param>
        public Target(string name, Shot[] shots)
        {
            Name = name;
            Shots = shots;
        }

        /// <summary>
        /// Gets the shot stored for a specific camera head.
        /// </summary>
        /// <param name="headId">The head id to search for the shot.</param>
        /// <returns>The camera heads shot information, if no shot is stored for the camera head then null is returned.</returns>
        public Shot GetShotForCamera(string headId)
        {
            return (from s in Shots where s.HeadId.Equals(headId) select s).SingleOrDefault();
        }

    }
}
