namespace Vitec.CameraHead.Models;

using System;

/// <summary>
///     Interface for Pan/Tilt robotic camera heads
/// </summary>
public interface ICameraHead
{
    /// <summary>
    ///     Gets the name of the robot, the name is unique.
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     Tells the head to move to a position
    /// </summary>
    /// <remarks>
    ///     WARNING: This is a synchronous call for interview purposes, the real call would be asynchronous.
    ///     What this means is that the call will not return until the robot stops moving. However events will still be raised
    ///     on another thread.
    ///     Other important points, if the head is moving it will stop before moving to the new position, even if the position
    ///     is the same.
    /// </remarks>
    /// <param name="position">The new position to send the head to.</param>
    void Move(Position position);

    void StopCamera();

    /// <summary>
    ///     Event raised when the head position changes.
    /// </summary>
    event EventHandler<CameraHeadPositionEventArgs> PositionChanged;

    /// <summary>
    ///     Event raised when the status of the head changes.
    /// </summary>
    event EventHandler<StatusChangedEventArgs> StatusChanged;
}