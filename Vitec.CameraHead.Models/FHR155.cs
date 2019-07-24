﻿namespace Vitec.CameraHead.Models {

    /// <summary>
    ///     Model for FHR-155 Heavy Payload Robotic Camera Head
    /// </summary>
    /// <remarks>
    ///     Pan Range 359 deg (39 def with end stops)
    ///     Tilt Range 345 deg
    ///     Max Pan/Tilt Speed 60 deg/sec
    ///     Min Pan/Tilt Speed 0.01 deg/sec
    /// </remarks>
    public class FHR155 : CameraHeadBase {

        /// <summary>
        /// Create a new instance of an FHR-35
        /// </summary>
        /// <param name="name"></param>
        /// <param name="panVelocity"></param>
        /// <param name="tiltVelocity"></param>
        public FHR155(string name, double panVelocity = 1, double tiltVelocity = 1) : base(name, panVelocity, tiltVelocity) {
            MinPosition = new Position(-179.5, -50);
            MaxPosition = new Position(179.5, 50);
            UpdateInterval = 100; // Update interval in milliseconds
        }

    }
}