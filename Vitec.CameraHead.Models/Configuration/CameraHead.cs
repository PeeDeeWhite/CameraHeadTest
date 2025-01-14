﻿namespace Vitec.CameraHead.Models.Configuration;

/// <summary>
///     Individual camera head and shot configuration data for loading into <see cref="Studio" /> instance
/// </summary>
public class CameraHead
{
    /// <summary>
    ///     The type of CameraHead to instantiate
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    ///     Set to true if the call to Move needs to be from an async task
    /// </summary>
    public bool WrapInAsyncCall { get; set; }

    /// <summary>
    ///     Get/Sets the Camera Head name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///     Pan Velocity in degrees/sec
    /// </summary>
    public double PanVelocity { get; set; }

    /// <summary>
    ///     Tilt Velocity in degrees/sec
    /// </summary>
    public double TiltVelocity { get; set; }

    /// <summary>
    ///     Gets/Sets list of Positions, one for each target
    /// </summary>
    public TargetPosition[] TargetPositions { get; set; }
}