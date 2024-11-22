namespace Vitec.CameraHead.Models;

/// <summary>
///     Immutable class representing the position of a camera head for a shot
/// </summary>
public sealed class Shot(string headId, Position position)
{
    public string HeadId { get; } = headId;

    public Position Position { get; } = position;
}