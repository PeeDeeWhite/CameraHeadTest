namespace Vitec.CameraHead.Models;

using System;

/// <summary>
///     A class representing the event arguments raised by a <see cref="CameraHeadStatus.Moving" /> event.
/// </summary>
public class CameraHeadPositionEventArgs(Position currentPosition, TimeSpan timeToShot) : EventArgs
{
    public Position CurrentPosition { get; } = currentPosition;

    public TimeSpan TimeToShot { get; } = timeToShot;
}