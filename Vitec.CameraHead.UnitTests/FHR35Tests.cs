﻿namespace Vitec.CameraHead.UnitTests {
    using System;
    using System.Threading;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Vitec.CameraHead.Models;

    [TestClass]
    public class FHR35Tests {

        [TestMethod]
        public void WhenPositionSet_ShouldRaiseEvents() {
            var cameraHead = new FHR35("Test");

            using (var monitoredSubject = cameraHead.Monitor()) {
                cameraHead.SetPosition(new Position(10.0, 10.0));
                 Thread.Sleep(150); // give time for OnPositionChanged to fire
                monitoredSubject.Should()
                    .Raise(nameof(FHR35.OnStatusChanged))
                    .WithArgs<StatusChangedEventArgs>(args => args.Status == CameraHeadStatus.Idle);
                monitoredSubject.Should()
                    .Raise(nameof(FHR35.OnPositionChanged))
                    .WithArgs<CameraHeadPositionEventArgs>(args => args.CurrentPosition.Equals(new Position(0.1, 0.1)));
            }
        }

        [TestMethod]
        public void WhenMovementPositive_ShouldCalcTimeToShot()
        {
            var cameraHead = new FHR35("Test");

            using (var monitoredSubject = cameraHead.Monitor())
            {
                cameraHead.SetPosition(new Position(10.0, 10.0));
                Thread.Sleep(150); // give time for OnPositionChanged to fire
                monitoredSubject.Should()
                    .Raise(nameof(FHR35.OnPositionChanged))
                    .WithArgs<CameraHeadPositionEventArgs>(args => args.TimeToShot == new TimeSpan(0,0,0,10));
            }
        }

        [TestMethod]
        public void WhenTargetPositionGreaterThanMaxRanges_ShouldUseMaxValues()
        {
            var cameraHead = new FHR35("Test", 200, 200);

            using (var monitoredSubject = cameraHead.Monitor())
            {
                cameraHead.SetPosition(new Position(200.0, 200.0));
                Thread.Sleep(10100); // give time for OnPositionChanged to fire
                monitoredSubject.Should()
                    .Raise(nameof(FHR35.OnPositionChanged))
                    .WithArgs<CameraHeadPositionEventArgs>(args => args.CurrentPosition.Equals(new Position(179.5, 172.5)));
            }
        }

        [TestMethod]
        public void WhenTargetPositionLessThanMinRanges_ShouldUseMinValues()
        {
            var cameraHead = new FHR35("Test", 200, 200);

            using (var monitoredSubject = cameraHead.Monitor())
            {
                cameraHead.SetPosition(new Position(-200.0, -200.0));
                Thread.Sleep(10100); // give time for OnPositionChanged to fire
                monitoredSubject.Should()
                    .Raise(nameof(FHR35.OnPositionChanged))
                    .WithArgs<CameraHeadPositionEventArgs>(args => args.CurrentPosition.Equals(new Position(-179.5, -172.5)));
            }
        }


    }
}