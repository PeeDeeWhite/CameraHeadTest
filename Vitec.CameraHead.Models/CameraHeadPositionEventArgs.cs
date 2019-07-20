namespace Vitec.CameraHead.Models {
    using System;

    /// <summary>
    ///     A class representing the event arguments raised by a <see cref="CameraHeadStatus.Moving" /> event.
    /// </summary>
    public class CameraHeadPositionEventArgs : EventArgs {

        /// <summary>
        ///     Initialises a new instance of the <see cref="CameraHeadPositionEventArgs" /> class.
        /// </summary>
        /// <param name="currentPosition">The current position of the robotic camera head.</param>
        /// <param name="timeToShot">The time until the shot will be on target.</param>
        public CameraHeadPositionEventArgs(Position currentPosition, TimeSpan timeToShot) {
            CurrentPosition = currentPosition;
            TimeToShot = timeToShot;
        }

        /// <summary>
        ///     Gets the current position of the robot.
        /// </summary>
        public Position CurrentPosition { get; }

        /// <summary>
        ///     Gets the time until the robot will be on-target.
        /// </summary>
        public TimeSpan TimeToShot { get; }
    }
}