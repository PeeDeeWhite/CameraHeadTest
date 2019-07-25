// ReSharper disable CompareOfFloatsByEqualityOperator  - For simplicity allow floating point equality

namespace Vitec.CameraHead.Models {

    /// <summary>
    ///   Immutable class representing a Pan/Tilt position in degrees
    /// </summary>
    public sealed class Position {

        /// <summary>
        ///     Initialises a new instance of the <see cref="Position" /> class.
        /// </summary>
        /// <param name="pan">The pan value.</param>
        /// <param name="tilt">The tilt value.</param>
        public Position(double pan, double tilt) {
            Pan = pan;
            Tilt = tilt;
        }

        /// <summary>
        ///     Gets the pan position.
        /// </summary>
        public double Pan { get; }

        /// <summary>
        ///     Gets the tilt position.
        /// </summary>
        public double Tilt { get; }

        /// <summary>
        ///     Custom GetHashCode using Prime number pattern
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() {
            unchecked // Overflow is fine, just wrap
            {
                var hash = 17;
                hash = hash * 23 + Pan.GetHashCode();
                hash = hash * 23 + Tilt.GetHashCode();

                return hash;
            }

#pragma warning disable 162 // Ignore unreachable code warning

            // Alternatively you can create an anonymous object
            // and call the Framework method which implements the same code.
            return new { Pan, Tilt }.GetHashCode();

            // In C#7 you can create a Tuple
            return (Pan, Tilt).GetHashCode();
        }

        /// <summary>
        ///  Custom Equals for <see cref="Position"/>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) {
            if (obj is Position otherPosition) {
                return Equals(otherPosition);
            }

            return false;
        }

        private bool Equals(Position other) {
            return other != null && other.Pan == Pan && other.Tilt == Tilt;
        }

        /// <summary>
        /// Override ToString to return Position as Pan and Tilt
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return $"[{Pan:N4}, {Tilt:N4}]";
        }
    }

}