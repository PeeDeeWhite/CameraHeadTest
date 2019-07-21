namespace Vitec.CameraHead.MotionTest {
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Extensions.Configuration;
    using Vitec.CameraHead.Models;
    using Vitec.CameraHead.Models.Configuration;

    /// <summary>
    ///     Static extension helper methods
    /// </summary>
    public static class Extensions {

        /// <summary>
        /// Create Studio from configuration data
        /// </summary>
        /// <param name="configuration"></param>
        /// <remarks>
        /// The configuration data was intentionally structured differently to the Studio model to demonstrate
        /// using extension methods and Linq to load and project the data from one format to another
        /// </remarks>
        /// <returns></returns>
        public static Studio CreateStudio(this IConfiguration configuration) {
            // Load studio data from configuration

            var targetNames = configuration
                .GetConfigurationSection<IEnumerable<string>>(Constants.Configuration.Targets);

            var cameraHeads = configuration
                .GetConfigurationSection<IEnumerable<CameraHead>>(Constants.Configuration.CameraHeads)
                .ToArray();

            return new Studio(
                targetNames.ToTargets(cameraHeads),
                cameraHeads.Select(x => x.ToCameraHead()));
        }

        private static ICameraHead ToCameraHead(this CameraHead cameraHead) {
            if (cameraHead == null) {
                throw new ArgumentNullException(nameof(cameraHead), "A camera head configuration is required");
            }

            if (string.IsNullOrWhiteSpace(cameraHead.Type)) {
                throw new ArgumentNullException(nameof(cameraHead.Type), "A camera head type name is required");
            }

            var cameraType = Type.GetType(cameraHead.Type);

            if (cameraType == null) {
                throw new InvalidOperationException($"Invalid camera head type {cameraHead.Type}");
            }


            // Build constructor arguments with Type.Missing for optional parameters which are not supplied
            var constructorArgs = new[] {
                cameraHead.Name,
                cameraHead.PanVelocity ?? Type.Missing,
                cameraHead.TiltVelocity ?? Type.Missing
            };

            return (ICameraHead)Activator.CreateInstance(cameraType,
                BindingFlags.CreateInstance |
                BindingFlags.Public |
                BindingFlags.Instance |
                BindingFlags.OptionalParamBinding,
                null,
                constructorArgs,
                CultureInfo.CurrentCulture);
        }

        private static IEnumerable<Target> ToTargets(this IEnumerable<string> targetNames, IEnumerable<CameraHead> cameraHeads) {
            if (targetNames == null) {
                throw new ArgumentNullException(nameof(targetNames), "Target Names are required");
            }

            if (cameraHeads == null)
            {
                throw new ArgumentNullException(nameof(cameraHeads), "Camera heads are required");
            }

            // Project configuration settings into Target creating Shots from position info on the Camera Heads
            // Example of using indexing in Linq queries
            return targetNames.Select((x, index) => 
                new Target(x, cameraHeads
                    .Select(y => new Shot(y.Name, y.ToPosition(index))).ToArray()));
        }

        /// <summary>
        /// Map nth target position to new instance of a Position
        /// </summary>
        /// <param name="cameraHead"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static Position ToPosition(this CameraHead cameraHead, int index) {
            var targetPosition = cameraHead.TargetPositions[index];
            return new Position(targetPosition.Pan, targetPosition.Tilt);
        }
    }
}