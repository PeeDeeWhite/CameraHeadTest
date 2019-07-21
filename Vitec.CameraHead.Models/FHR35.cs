﻿namespace Vitec.CameraHead.Models {

    /// <summary>
    ///     Model for FHR-35 Robotic Camera Head
    /// </summary>
    /// <remarks>
    ///     Pan Range 359 deg (39 def with end stops)
    ///     Tilt Range +/-50 deg (payload dependent)
    ///     Max Pan/Tilt Speed 60 deg/sec
    ///     Min Pan/Tilt Speed 0.05 deg/sec
    /// </remarks>
    public class FHR35 : CameraHeadBase {

        /// <summary>
        /// Create a new instance of an FHR-35
        /// </summary>
        /// <param name="name"></param>
        /// <param name="panVelocity"></param>
        /// <param name="tiltVelocity"></param>
        public FHR35(string name, double panVelocity = 30, double tiltVelocity = 30) : base(name, panVelocity, tiltVelocity) {
            MinPanRange = -179.5;
            MaxPanRange = 179.5;
            MinTiltRange = -50;
            MaxTiltRange = 50;
            UpdateInterval = 100; // Update interval in milliseconds
        }

    }
}