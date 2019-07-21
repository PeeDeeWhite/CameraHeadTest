namespace Vitec.CameraHead.UnitTests {
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Vitec.CameraHead.Models.Configuration;
    using Vitec.CameraHead.MotionTest;

    [TestClass]
    public class StudioConfigurationTests {

        [TestMethod]
        public void ConfigurationBuilder_ShouldLoadTargetNamesConfiguration()
        {
            var configuration = Configuration.BuildConfiguration(Assembly.GetExecutingAssembly());
            var targets = configuration
                .GetConfigurationSection<IEnumerable<string>>(Constants.Configuration.Targets)
                .ToArray();

            targets.Should().NotBeNull();
            targets.Should().NotBeEmpty();
            targets.Should().HaveCount(4);
            targets.Should().Contain(x => x == "Target 1");
        }

        [TestMethod]
        public void ConfigurationBuilder_ShouldLoadCameraHeadConfiguration() {
            var configuration = Configuration.BuildConfiguration(Assembly.GetExecutingAssembly());
            var cameraHeads =  configuration
                .GetConfigurationSection<IEnumerable<CameraHead>>(Constants.Configuration.CameraHeads)
                .ToArray();

            cameraHeads.Should().NotBeNull();
            cameraHeads.Should().NotBeEmpty();
            cameraHeads.Should().HaveCount(5);
            cameraHeads.Should().Contain(x => x.Name == "Test Head - Fixed pan/tilt velocity");
            cameraHeads[0].TargetPositions.Should().NotBeNull();
            cameraHeads[0].TargetPositions.Should().NotBeEmpty();
            cameraHeads[0].TargetPositions[0].Should().BeEquivalentTo(new TargetPosition { Pan = 5.0, Tilt = 4.4 });
        }

        [TestMethod]
        public void ConfigurationExtensions_ShouldCreateStudioFromConfiguration()
        {
            var configuration = Configuration.BuildConfiguration(Assembly.GetExecutingAssembly());

            var studio = configuration.CreateStudio();

            studio.Should().NotBeNull();
            studio.Targets.Should().NotBeNull();
            studio.Targets.Should().NotBeEmpty();
            studio.Targets.Should().HaveCount(4);
        }

    }
}