namespace Vitec.CameraHead.Models;

using System.Linq;

/// <summary>
///     Immutable class representing a position within a studio or conference hall, this may be a weather map, a clock or a
///     seat.
/// </summary>
/// <remarks>
///     A target can have stored one or more shots, with each shot representing a single camera head position (pan/tilt),
///     used to point to a target.
/// </remarks>
public sealed class Target(string name, Shot[] shots)
{
    public string Name { get; } = name;

    public Shot[] Shots { get; } = shots;

    /// <summary>
    ///     Gets the shot stored for a specific camera head.
    /// </summary>
    /// <param name="headId">The head id to search for the shot.</param>
    /// <returns>The camera heads shot information, if no shot is stored for the camera head then null is returned.</returns>
    public Shot GetShotForCamera(string headId)
    {
        return (from s in Shots where s.HeadId.Equals(headId) select s).SingleOrDefault();
    }
}