namespace Vitec.CameraHead.UnitTests;

using System;
using FluentAssertions;
using Microsoft.Extensions.Time.Testing;
using Models;
using Xunit;

/// <summary>
///     Tests from FHR35 and FHR155 repeated for the decompiled Test Camera head.
///     NOTE: All written to expect failure as the Test Camera Head does not have the improved logic of the FHR35 and FHR155.
/// </summary>
public class CameraHeadTests
{
    private const double PanVelocity = 0.50123;
    private const double TiltVelocity = 0.50123;

    [Fact]
    public void WhenPositionSet_ShouldFailToRaiseEventsWithExpectedValues()
    {
        var fakeTimeProvider = new FakeTimeProvider();
        var cameraHead = new TestCameraHead("Test", PanVelocity, TiltVelocity, fakeTimeProvider);

        using (var monitoredSubject = cameraHead.Monitor())
        {
            cameraHead.Move(new Position(10.0, 10.0));
            fakeTimeProvider.Advance(TimeSpan.FromMilliseconds(150));
            monitoredSubject.Should().Raise(nameof(TestCameraHead.StatusChanged))
                .WithArgs<StatusChangedEventArgs>(args => args.Status != CameraHeadStatus.Idle);
            monitoredSubject.Should()
                .Raise(nameof(TestCameraHead.PositionChanged))
                .WithArgs<CameraHeadPositionEventArgs>(args => !args.CurrentPosition.Equals(new Position(0.1, 0.1)));
        }
    }

    [Fact]
    public void WhenMovementPositive_ShouldFailToCalcCorrectTimeToShot()
    {
        var fakeTimeProvider = new FakeTimeProvider();
        var cameraHead = new TestCameraHead("Test", 1, 1, fakeTimeProvider);

        using (var monitoredSubject = cameraHead.Monitor())
        {
            cameraHead.Move(new Position(10.0, 10.0));
            fakeTimeProvider.Advance(TimeSpan.FromMilliseconds(150));
            monitoredSubject.Should()
                .Raise(nameof(TestCameraHead.PositionChanged))
                .WithArgs<CameraHeadPositionEventArgs>(args => args.TimeToShot != new TimeSpan(0, 0, 0, 10));
        }
    }

    [Fact]
    public void WhenTargetPositionGreaterThanMaxRanges_ShouldFaileToUseMaxValues()
    {
        var fakeTimeProvider = new FakeTimeProvider();
        var cameraHead = new TestCameraHead("Test", 200, 200, fakeTimeProvider);

        using (var monitoredSubject = cameraHead.Monitor())
        {
            cameraHead.Move(new Position(200.0, 200.0));
            
            fakeTimeProvider.Advance(TimeSpan.FromMilliseconds(10100));
            monitoredSubject.Should()
                .Raise(nameof(TestCameraHead.PositionChanged))
                .WithArgs<CameraHeadPositionEventArgs>(args => !args.CurrentPosition.Equals(new Position(179.5, 172.5)));
        }
    }

    [Fact]
    public void WhenTargetPositionLessThanMinRanges_ShouldFailToUseMinValues()
    {
        var fakeTimeProvider = new FakeTimeProvider();
        var cameraHead = new TestCameraHead("Test", 200, 200, fakeTimeProvider);

        using (var monitoredSubject = cameraHead.Monitor())
        {
            cameraHead.Move(new Position(-200.0, -200.0));
            fakeTimeProvider.Advance(TimeSpan.FromMilliseconds(10100));
            monitoredSubject.Should()
                .Raise(nameof(TestCameraHead.PositionChanged))
                .WithArgs<CameraHeadPositionEventArgs>(args => !args.CurrentPosition.Equals(new Position(-179.5, -172.5)));
        }
    }
}