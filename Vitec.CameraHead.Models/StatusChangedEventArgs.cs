namespace Vitec.CameraHead.Models
{
    using System;

    /// <summary>
    /// Immutable class representing the event arguments raised by a <see cref="ICameraHead.OnStatusChanged"/> event.
    /// </summary>
    public sealed class StatusChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets a value indicating the current robot state.
        /// </summary>
        public CameraHeadStatus Status { get; }

        /// <summary>
        /// Initialises a new instance of the <see cref="StatusChangedEventArgs"/> class.
        /// </summary>
        /// <param name="status">The new status.</param>
        public StatusChangedEventArgs(CameraHeadStatus status)
        {
            Status = status;
        }
    }
}
