namespace Vitec.CameraHead.Models
{
    /// <summary>
    /// Immutable class representing a the position of a camera head for a shot 
    /// </summary>
    public sealed class Shot
    {
        /// <summary>
        /// Gets the head id for the shot.
        /// </summary>
        public string HeadId {get; }

        /// <summary>
        /// Gets the position for the head shot.
        /// </summary>
        public Position Position {get; }

        /// <summary>
        /// Initialises a new instance of the <see cref="Shot"/> class.
        /// </summary>
        /// <param name="headId">The id of the camera head that this shot is for.</param>
        /// <param name="position">The shot camera head position.</param>
        public Shot(string headId, Position position)
        {
            HeadId = headId;
            Position = position;
        }
    }
}
