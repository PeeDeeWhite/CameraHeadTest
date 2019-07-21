namespace Vitec.CameraHead.Models.Configuration {
    /// <summary>
    ///     Used to load Target positions from configuration
    ///     As <see cref="Position" /> does not have public setters or parameterless constructor
    ///     the configuration builder can deserialize the appsettings into it so we use this class
    ///     instead.
    /// </summary>
    public class TargetPosition {

        /// <summary>
        /// Gets/sets the pan position.
        /// </summary>
        public double Pan { get; set; }

        /// <summary>
        /// Gets/sets the tilt position.
        /// </summary>
        public double Tilt { get; set; }
    }
}