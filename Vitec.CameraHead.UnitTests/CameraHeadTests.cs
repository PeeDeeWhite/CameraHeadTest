namespace Vitec.CameraHead.UnitTests {
    using System;
    using System.Threading;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Vitec.CameraHead.Models;

    /// <summary>
    /// Tests from FHR35 and FHR155 repeated for the decompiled Test Camera head.
    /// NOTE: All fail
    /// </summary>
    [TestClass]
    public class CameraHeadTests
    {

        [TestMethod]
        public void WhenPositionSet_ShouldRaiseEvents() {
            var cameraHead = new TestCameraHead("Test");

            using (var monitoredSubject = cameraHead.Monitor()) {
                cameraHead.SetPosition(new Position(10.0, 10.0));
                 Thread.Sleep(150); // give time for OnPositionChanged to fire
                monitoredSubject.Should()
                    .Raise(nameof(TestCameraHead.OnStatusChanged))
                    .WithArgs<StatusChangedEventArgs>(args => args.Status == CameraHeadStatus.Idle);
                monitoredSubject.Should()
                    .Raise(nameof(TestCameraHead.OnPositionChanged))
                    .WithArgs<CameraHeadPositionEventArgs>(args => args.CurrentPosition.Equals(new Position(0.1, 0.1)));
            }
        }

        [TestMethod]
        public void WhenMovementPositive_ShouldCalcTimeToShot()
        {
            var cameraHead = new TestCameraHead("Test");

            using (var monitoredSubject = cameraHead.Monitor())
            {
                cameraHead.SetPosition(new Position(10.0, 10.0));
                Thread.Sleep(150); // give time for OnPositionChanged to fire
                monitoredSubject.Should()
                    .Raise(nameof(TestCameraHead.OnPositionChanged))
                    .WithArgs<CameraHeadPositionEventArgs>(args => args.TimeToShot == new TimeSpan(0,0,0,10));
            }
        }

        [TestMethod]
        public void WhenTargetPositionGreaterThanMaxRanges_ShouldUseMaxValues()
        {
            var cameraHead = new TestCameraHead("Test", 200, 200);

            using (var monitoredSubject = cameraHead.Monitor())
            {
                cameraHead.SetPosition(new Position(200.0, 200.0));
                Thread.Sleep(10100); // give time for OnPositionChanged to fire
                monitoredSubject.Should()
                    .Raise(nameof(TestCameraHead.OnPositionChanged))
                    .WithArgs<CameraHeadPositionEventArgs>(args => args.CurrentPosition.Equals(new Position(179.5, 172.5)));
            }
        }

        [TestMethod]
        public void WhenTargetPositionLessThanMinRanges_ShouldUseMinValues()
        {
            var cameraHead = new TestCameraHead("Test", 200, 200);

            using (var monitoredSubject = cameraHead.Monitor())
            {
                cameraHead.SetPosition(new Position(-200.0, -200.0));
                Thread.Sleep(10100); // give time for OnPositionChanged to fire
                monitoredSubject.Should()
                    .Raise(nameof(TestCameraHead.OnPositionChanged))
                    .WithArgs<CameraHeadPositionEventArgs>(args => args.CurrentPosition.Equals(new Position(-179.5, -172.5)));
            }
        }


    }
}