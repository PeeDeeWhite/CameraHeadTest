namespace Vitec.CameraHead.UnitTests {
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Vitec.CameraHead.Models;
    using Vitec.CameraHead.MotionTest;

    [TestClass]
    public class ToDistanceTests {

        [TestMethod]
        public void WhenCurrentAndDestinationPositive_ShouldCalcDistance() {

            var currentPosition = new Position(10, 10);
            var destination = new Position(20, 20);

            currentPosition.DistanceTo(destination).Should().Be(20);
        }

        [TestMethod]
        public void WhenCurrentAndDestinationNegative_ShouldCalcDistance()
        {

            var currentPosition = new Position(-10, -10);
            var destination = new Position(-20, -20);

            currentPosition.DistanceTo(destination).Should().Be(20);
        }

        [TestMethod]
        public void WhenCurrentPositiveAndDestinationNegative_ShouldCalcDistance()
        {

            var currentPosition = new Position(10, 10);
            var destination = new Position(-20, -20);

            currentPosition.DistanceTo(destination).Should().Be(60);
        }

        [TestMethod]
        public void WhenCurrentNegativeAndDestinationPositive_ShouldCalcDistance()
        {

            var currentPosition = new Position(-10, -10);
            var destination = new Position(20, 20);

            currentPosition.DistanceTo(destination).Should().Be(60);
        }

        [TestMethod]
        public void WhenValuesMixedBetweenPositiveAndNegative_ShouldCalcDistance()
        {

            var currentPosition = new Position(-10, 10);
            var destination = new Position(20, -20);

            currentPosition.DistanceTo(destination).Should().Be(60);
        }

    }
}