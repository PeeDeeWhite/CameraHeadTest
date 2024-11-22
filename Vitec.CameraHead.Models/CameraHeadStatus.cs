namespace Vitec.CameraHead.Models;

/// <summary>
///     The status of the Camera head.
/// </summary>
public enum CameraHeadStatus
{
    /// <summary>
    ///     Not doing anything
    /// </summary>
    Idle,

    /// <summary>
    ///     Moving.
    /// </summary>
    Moving,

    /// <summary>
    ///     Disabled, either due to an error or by the user.
    /// </summary>
    Disabled
}