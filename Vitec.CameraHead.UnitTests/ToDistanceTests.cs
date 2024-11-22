namespace Vitec.CameraHead.UnitTests;

using FluentAssertions;
using Models;
using MotionTest;
using Xunit;

public class ToDistanceTests
{
    [Fact]
    public void WhenCurrentAndDestinationPositive_ShouldCalcDistance()
    {
        var currentPosition = new Position(10, 10);
        var destination = new Position(20, 20);

        currentPosition.DistanceTo(destination).Should().Be(20);
    }

    [Fact]
    public void WhenCurrentAndDestinationNegative_ShouldCalcDistance()
    {
        var currentPosition = new Position(-10, -10);
        var destination = new Position(-20, -20);

        currentPosition.DistanceTo(destination).Should().Be(20);
    }

    [Fact]
    public void WhenCurrentPositiveAndDestinationNegative_ShouldCalcDistance()
    {
        var currentPosition = new Position(10, 10);
        var destination = new Position(-20, -20);

        currentPosition.DistanceTo(destination).Should().Be(60);
    }

    [Fact]
    public void WhenCurrentNegativeAndDestinationPositive_ShouldCalcDistance()
    {
        var currentPosition = new Position(-10, -10);
        var destination = new Position(20, 20);

        currentPosition.DistanceTo(destination).Should().Be(60);
    }

    [Fact]
    public void WhenValuesMixedBetweenPositiveAndNegative_ShouldCalcDistance()
    {
        var currentPosition = new Position(-10, 10);
        var destination = new Position(20, -20);

        currentPosition.DistanceTo(destination).Should().Be(60);
    }
}