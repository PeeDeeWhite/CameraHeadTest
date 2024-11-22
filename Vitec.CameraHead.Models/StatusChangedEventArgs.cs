namespace Vitec.CameraHead.Models;

using System;

/// <summary>
///     Immutable class representing the event arguments raised by a <see cref="ICameraHead.StatusChanged" /> event.
/// </summary>
public sealed class StatusChangedEventArgs(CameraHeadStatus status) : EventArgs
{
    public CameraHeadStatus Status { get; } = status;
}