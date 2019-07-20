namespace Vitec.CameraHead.Models {
    using System.Collections.Generic;

    /// <summary>
    /// Represents a Studio, a space containing a number of camera heads <see cref="ICameraHead"/> to test
    /// and a number of targets <see cref="Target"/> for them to aim at
    /// </summary>
    /// <remarks>
    /// Reverse engineered from Interview1 namespace
    /// </remarks>
    public sealed class Studio {

        private readonly List<Target> _targets = new List<Target>()
        {
            new Target("Target 1", new[]
            {
                new Shot("Head 1", new Position(5.0, 4.4)),
                new Shot("Head 2", new Position(1.5, 4.4)),
                new Shot("Head 3", new Position(2.5, 14.4)),
                new Shot("Head 4", new Position(7.5, 4.4)),
                new Shot("Head 5", new Position(13.5, 4.4))
            }),
            new Target("Target 2", new[]
            {
                new Shot("Head 1", new Position(13.5, 1.1)),
                new Shot("Head 2", new Position(3.5, 1.1)),
                new Shot("Head 3", new Position(4.5, 11.1)),
                new Shot("Head 4", new Position(7.5, 1.1)),
                new Shot("Head 5", new Position(20.5, 1.1))
            }),
            new Target("Target 3", new[]
            {
                new Shot("Head 1", new Position(17.5, 14.2)),
                new Shot("Head 2", new Position(5.5, 14.2)),
                new Shot("Head 3", new Position(6.5, 24.2)),
                new Shot("Head 4", new Position(9.5, 14.2)),
                new Shot("Head 5", new Position(24.5, 14.2))
            }),
            new Target("Target 4", new[]
            {
                new Shot("Head 1", new Position(-3.5, 34.4)),
                new Shot("Head 2", new Position(-5.5, 34.4)),
                new Shot("Head 3", new Position(-13.5, 44.4)),
                new Shot("Head 4", new Position(-15.5, 34.4)),
                new Shot("Head 5", new Position(-25.5, 34.4))
            })
        };

        private readonly List<ICameraHead> _robots;

        public Studio() {
            _robots = new List<ICameraHead>() {
                new FHR35("FHR-35 Max Pan/Tilt", 60, 60),
                new TestCameraHead("Test Head 1 - Fixed pan/tilt velocity"),
                new FHR35("FHR-35 Default Pan/Tilt"),
                new TestCameraHead("Test Head 2 - Fixed pan/tilt velocity"),
                new FHR35("FHR-35 Min Pan/Tilt", 0.01, 0.01),
            };
        }

        public IEnumerable<ICameraHead> Robots => _robots;

        public IEnumerable<Target> Targets => _targets;
    }
}