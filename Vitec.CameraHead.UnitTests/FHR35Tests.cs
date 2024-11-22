namespace Vitec.CameraHead.UnitTests;

using System;
using FluentAssertions;
using Microsoft.Extensions.Time.Testing;
using Models;
using Xunit;

public class FHR35Tests
{
    [Fact]
    public void WhenPositionSet_ShouldRaiseEvents()
    {
        var fakeTimeProvider = new FakeTimeProvider();
        var cameraHead = new FHR35("Test", 1, 1, fakeTimeProvider);

        using (var monitoredSubject = cameraHead.Monitor())
        {
            cameraHead.Move(new Position(10.0, 10.0));
            fakeTimeProvider.Advance(TimeSpan.FromMilliseconds(150));
            monitoredSubject.Should()
                .Raise(nameof(FHR35.StatusChanged))
                .WithArgs<StatusChangedEventArgs>(args => args.Status == CameraHeadStatus.Idle);
            monitoredSubject.Should()
                .Raise(nameof(FHR35.PositionChanged))
                .WithArgs<CameraHeadPositionEventArgs>(args => args.CurrentPosition.Equals(new Position(0.1, 0.1)));
        }
    }

    [Fact]
    public void WhenMovementPositive_ShouldCalcTimeToShot()
    {
        var fakeTimeProvider = new FakeTimeProvider();
        var cameraHead = new FHR35("Test", 1, 1, fakeTimeProvider);

        using (var monitoredSubject = cameraHead.Monitor())
        {
            cameraHead.Move(new Position(10.0, 10.0));
            fakeTimeProvider.Advance(TimeSpan.FromMilliseconds(150));
            monitoredSubject.Should()
                .Raise(nameof(FHR35.PositionChanged))
                .WithArgs<CameraHeadPositionEventArgs>(args => args.TimeToShot == new TimeSpan(0, 0, 0, 10));
        }
    }

    [Fact]
    public void WhenTargetPositionGreaterThanMaxRanges_ShouldUseMaxValues()
    {
        var fakeTimeProvider = new FakeTimeProvider();
        var cameraHead = new FHR35("Test", 200, 200, fakeTimeProvider);

        using (var monitoredSubject = cameraHead.Monitor())
        {
            cameraHead.Move(new Position(200.0, 200.0));
            fakeTimeProvider.Advance(TimeSpan.FromMilliseconds(10100));
            monitoredSubject.Should()
                .Raise(nameof(FHR35.PositionChanged))
                .WithArgs<CameraHeadPositionEventArgs>(args => args.CurrentPosition.Equals(new Position(179.5, 172.5)));
        }
    }

    [Fact]
    public void WhenTargetPositionLessThanMinRanges_ShouldUseMinValues()
    {
        var fakeTimeProvider = new FakeTimeProvider();
        var cameraHead = new FHR35("Test", 200, 200, fakeTimeProvider);

        using (var monitoredSubject = cameraHead.Monitor())
        {
            cameraHead.Move(new Position(-200.0, -200.0));
            fakeTimeProvider.Advance(TimeSpan.FromMilliseconds(10100));
            monitoredSubject.Should()
                .Raise(nameof(FHR35.PositionChanged))
                .WithArgs<CameraHeadPositionEventArgs>(args => args.CurrentPosition.Equals(new Position(-179.5, -172.5)));
        }
    }
}