namespace Vitec.CameraHead.Models {
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents a Studio, a space containing a number of camera heads <see cref="ICameraHead"/> to test
    /// and a number of targets <see cref="Target"/> for them to aim at
    /// </summary>
    /// <remarks>
    /// Reverse engineered from Interview1 namespace
    /// </remarks>
    public sealed class Studio {

        private readonly List<Target> _targets;

        private readonly List<ICameraHead> _cameraHeads;

        /// <summary>
        /// Create new instance passing in Camera Heads and Targets
        /// </summary>
        /// <param name="cameraHeads"></param>
        /// <param name="targets"></param>
        public Studio(IEnumerable<Target> targets, IEnumerable<ICameraHead> cameraHeads) {
            _targets = targets == null ? new List<Target>() : targets.ToList();
            _cameraHeads = cameraHeads == null ? new List<ICameraHead>() : cameraHeads.ToList();
        }

        /// <summary>
        /// Readonly Collection of Camera Heads to test
        /// </summary>
        public IEnumerable<ICameraHead> CameraHeads => _cameraHeads;

        /// <summary>
        /// Readonly collection of Targets to aim Camera Heads at
        /// </summary>
        public IEnumerable<Target> Targets => _targets;
    }
}