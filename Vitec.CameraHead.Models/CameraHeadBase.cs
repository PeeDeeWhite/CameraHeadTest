// ReSharper disable CompareOfFloatsByEqualityOperator 

namespace Vitec.CameraHead.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

/// <summary>
///     Base class for Camera Heads
/// </summary>
public abstract class CameraHeadBase : ICameraHead
{
    private readonly List<CancellationTokenSource> _cancellationTokenSources = [];
    private readonly double _panVelocity; // degrees/second
    private readonly double _tiltVelocity; // degrees/second
    private Position _currentPosition;
    private protected Position MaxPosition; // Maximum Pan and Tilt range
    private protected Position MinPosition; // Minimum Pan and Tilt range
    private protected int UpdateInterval; // Update interval in milliseconds
    private readonly ITimer _timer;
    private readonly PositionState _positionState;

    protected CameraHeadBase(string name, double panVelocity, double tiltVelocity, TimeProvider timeProvider)
    {
        Name = name;

        // Assumes pan / tilt velocity not changed during life-cycle of Camera Head    
        _panVelocity = panVelocity;
        _tiltVelocity = tiltVelocity;
        _positionState = new PositionState();
        _timer = timeProvider.CreateTimer(
            callback: CalcNewPosition,
            state: _positionState,
            dueTime: Timeout.InfiniteTimeSpan,
            period: Timeout.InfiniteTimeSpan);
        // Assume Head initialized to home location
        _currentPosition = new Position(0.0, 0.0);
    }

    public event EventHandler<CameraHeadPositionEventArgs> PositionChanged;
    public event EventHandler<StatusChangedEventArgs> StatusChanged;

    public string Name { get; }

    public void Move(Position position)
    {
        // Cancel any task in progress
        foreach (var cts in _cancellationTokenSources.Where(x => !x.IsCancellationRequested))
        {
            cts.Cancel();
        }

        // Register head stopping before moving to new position
        OnStatusChanged(new StatusChangedEventArgs(CameraHeadStatus.Idle));

        if (_currentPosition.Equals(position)) return;

        var cancellationTokenSource = new CancellationTokenSource();
        _cancellationTokenSources.Add(cancellationTokenSource);

        MoveToDestination(_currentPosition, AdjustForLimits(position, MinPosition, MaxPosition), cancellationTokenSource.Token);
    }

    public void StopCamera()
    {
        _timer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        OnStatusChanged(new StatusChangedEventArgs(CameraHeadStatus.Idle));
        DebugHelpers.Log($"Camera Head {Name} - Complete");
    }

    /// <summary>
    ///     Limit destination position to min and max Pan and Tilt parameters
    /// </summary>
    /// <param name="position"></param>
    /// <param name="minPosition"></param>
    /// <param name="maxPosition"></param>
    /// <returns></returns>
    private Position AdjustForLimits(Position position, Position minPosition, Position maxPosition)
    {
        var destinationPan = position.Pan >= 0 ? Math.Min(position.Pan, maxPosition.Pan) : Math.Max(position.Pan, minPosition.Pan);
        var destinationTilt = position.Tilt >= 0 ? Math.Min(position.Tilt, maxPosition.Tilt) : Math.Max(position.Tilt, minPosition.Tilt);

        return new Position(destinationPan, destinationTilt);
    }

    /// <summary>
    ///     Simulate moving to new position using the pan and tilt velocity
    ///     Start timer and raise events every UpdateInterval
    /// </summary>
    /// <param name="current"></param>
    /// <param name="destination"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private void MoveToDestination(Position current, Position destination, CancellationToken cancellationToken)
    {
        var panVector = CalcVectorPerInterval(current.Pan, destination.Pan, _panVelocity);
        var tiltVector = CalcVectorPerInterval(current.Tilt, destination.Tilt, _tiltVelocity);
        DebugHelpers.Log($"Camera Head {Name} - Current {current} to Target {destination} - Pan Velocity {_panVelocity} Pan Vector {panVector:N2} Tilt Velocity {_tiltVelocity} Tilt Vector {tiltVector:N2} Interval {UpdateInterval}");

        _positionState.Destination = destination;
        _positionState.PanVector = panVector;
        _positionState.TiltVector = tiltVector;
        _positionState.CancellationToken = cancellationToken;
        _timer.Change(TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(UpdateInterval));
    }

    /// <summary>
    ///     Calculate the direction of travel and determine the distance traveled in degrees per interval at the given
    ///     velocity.
    ///     This gives the vector for movement to be added each interval to the current position
    /// </summary>
    /// <param name="current"></param>
    /// <param name="destination"></param>
    /// <param name="velocity">degrees / sec</param>
    /// <returns></returns>
    private double CalcVectorPerInterval(double current, double destination, double velocity)
    {
        var toTravel = destination - current;

        // Calc the distance we can travel each interval and multiply by the direction 
        return toTravel == 0.0 ? 0.0 : (destination > current ? 1 : -1) * (velocity * (UpdateInterval / 1000.00));
    }

    private bool MovementCompleted(Position destination)
    {
        // We can test for equality because the CalcNewPosition will stop exactly at destination 
        return _currentPosition.Equals(destination);
    }

    private void CalcNewPosition(object state)
    {
        var positionState = (PositionState)state;
        if (positionState.CancellationToken.IsCancellationRequested)
        {
            StopCamera();
        }

        OnStatusChanged(new StatusChangedEventArgs(CameraHeadStatus.Moving));

        var newPosition = new Position(
            IncrementByVector(_currentPosition.Pan, positionState.PanVector, positionState.Destination.Pan),
            IncrementByVector(_currentPosition.Tilt, positionState.TiltVector, positionState.Destination.Tilt));

        DebugHelpers.Log($"Camera Head {Name} - New {newPosition}");

        var panTimeToDestination = Math.Abs(positionState.Destination.Pan - _currentPosition.Pan) / _panVelocity;
        var tiltTimeToDestination = Math.Abs(positionState.Destination.Tilt - _currentPosition.Tilt) / _tiltVelocity;

        // Determine if Pan or Tilt movement wil take the longest.
        var timeToDestination = panTimeToDestination > tiltTimeToDestination
            ? panTimeToDestination
            : tiltTimeToDestination;

        OnPositionChanged(new CameraHeadPositionEventArgs(newPosition, TimeSpan.FromSeconds(timeToDestination)));

        _currentPosition = newPosition;

        if (MovementCompleted(positionState.Destination))
        {
             StopCamera();
        }
    }

    private double IncrementByVector(double currentPosition, double vector, double destination)
    {
        var newPosition = currentPosition + vector;
        if (vector > 0) return newPosition >= destination ? destination : newPosition;
        return newPosition <= destination ? destination : newPosition;
    }

    private void OnStatusChanged(StatusChangedEventArgs e)
    {
        DebugHelpers.Log($"Camera Head {Name} - Status {e.Status}");

        StatusChanged?.Invoke(this, e);
    }

    private void OnPositionChanged(CameraHeadPositionEventArgs e)
    {
        PositionChanged?.Invoke(this, e);
    }
    
    private class PositionState
    {
        public Position Destination { get; set; }
        public double PanVector { get; set; }
        public double TiltVector { get; set; }
        public CancellationToken CancellationToken { get; set; }
    }
}