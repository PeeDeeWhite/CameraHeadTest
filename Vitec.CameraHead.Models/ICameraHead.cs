namespace Vitec.CameraHead.Models {
    using System;

    /// <summary>
    ///     Interface for Pan/Tilt robotic camera heads
    /// </summary>
    /// <remarks>
    ///   NOTE: Standard .NET event naming conventions means events should not have the On prefix
    ///   The handler in the subscriber should have the On prefix
    /// </remarks>
    public interface ICameraHead {

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
        void SetPosition(Position position);

        /// <summary>
        ///     Event raised when the head position changes.
        /// </summary>
        event EventHandler<CameraHeadPositionEventArgs> OnPositionChanged;

        /// <summary>
        ///     Event raised when the status of the head changes.
        /// </summary>
        event EventHandler<StatusChangedEventArgs> OnStatusChanged;
    }
}