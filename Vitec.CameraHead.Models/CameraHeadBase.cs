// ReSharper disable CompareOfFloatsByEqualityOperator 
namespace Vitec.CameraHead.Models {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Base class for Camera Heads
    /// </summary>
    public abstract class CameraHeadBase : ICameraHead {
        private readonly double _panVelocity; // degrees/second
        private readonly double _tiltVelocity; // degrees/second
        private protected Position MinPosition; // Minimum Pan and Tilt range
        private protected Position MaxPosition; // Maximum Pan and Tilt range
        private protected int UpdateInterval; // Update interval in milliseconds
        private readonly List<CancellationTokenSource> _cancellationTokenSources = new List<CancellationTokenSource>();
        private Position _currentPosition;

        public event EventHandler<CameraHeadPositionEventArgs> OnPositionChanged;
        public event EventHandler<StatusChangedEventArgs> OnStatusChanged;

        protected CameraHeadBase(string name, double panVelocity, double tiltVelocity) {
            Name = name;

            // Assumes pan / tilt velocity not changed during life-cycle of Camera Head    
            _panVelocity = panVelocity;
            _tiltVelocity = tiltVelocity;

            // Assume Head initialized to home location
            _currentPosition = new Position(0.0, 0.0);
        }


        public string Name { get; }

        /// <inheritdoc />
        /// <remarks>
        ///     We are constrained by the interface to use void.
        ///     In order to use a Task on a background thread we have to declare the method as async void
        ///     which ideally should be avoided.  However as the SetPosition method does not need to know the outcome
        ///     of the async MoveToPosition method we can use this in this situation and by setting ConfigureAwait
        ///     to false we can avoid blocking the synchronization context.
        /// </remarks>
        public async void SetPosition(Position position) {
            // Cancel any task in progress
            foreach (var cts in _cancellationTokenSources.Where(x => !x.IsCancellationRequested)) {
                    cts.Cancel();
            }

            // Register head stopping before moving to new position
            RaiseOnStatusChanged(new StatusChangedEventArgs(CameraHeadStatus.Idle));

            if (_currentPosition.Equals(position)) {
                return;
            }

            var cancellationTokenSource = new CancellationTokenSource();
            _cancellationTokenSources.Add(cancellationTokenSource);

            try {
                await MoveToPosition(_currentPosition, AdjustForLimits(position, MinPosition, MaxPosition), cancellationTokenSource.Token)
                    .ConfigureAwait(false);
            }
            catch (OperationCanceledException) {
                Debug.WriteLine($"{DateTime.UtcNow:T} {nameof(SetPosition)} Camera Head {Name} - Cancelled");
            }
        }

        /// <summary>
        ///  Limit destination position to min and max Pan and Tilt parameters
        /// </summary>
        /// <param name="position"></param>
        /// <param name="minPosition"></param>
        /// <param name="maxPosition"></param>
        /// <returns></returns>
        private Position AdjustForLimits(Position position, Position minPosition, Position maxPosition) {
            var destinationPan = position.Pan >= 0 ? Math.Min(position.Pan, maxPosition.Pan) : Math.Max(position.Pan, minPosition.Pan);
            var destinationTilt = position.Tilt >= 0 ? Math.Min(position.Tilt, maxPosition.Tilt) : Math.Max(position.Tilt, minPosition.Tilt);

            return new Position(destinationPan, destinationTilt);
        }

        /// <summary>
        ///     Simulate moving to new position using the pan and tilt velocity
        ///     Raise events every UpdateInterval
        /// </summary>
        /// <param name="current"></param>
        /// <param name="destination"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task MoveToPosition(Position current, Position destination, CancellationToken cancellationToken) {

            var panVector = CalcVectorPerInterval(current.Pan, destination.Pan, _panVelocity);
            var tiltVector = CalcVectorPerInterval(current.Tilt, destination.Tilt, _tiltVelocity);
            Debug.WriteLine($"{DateTime.UtcNow:T} {nameof(MoveToPosition)} Camera Head {Name} - Current {current} to Target {destination} - Pan Velocity {_panVelocity} Pan Vector {panVector:N2} Tilt Velocity {_tiltVelocity} Tilt Vector {tiltVector:N2} Interval {UpdateInterval}");

            while (!MovementCompleted(destination)) {
                if (cancellationToken.IsCancellationRequested) {
                    cancellationToken.ThrowIfCancellationRequested();
                }

                _currentPosition = await CalcNewPosition(panVector, tiltVector, destination);
            }

            RaiseOnStatusChanged(new StatusChangedEventArgs(CameraHeadStatus.Idle));
            Debug.WriteLine($"{DateTime.UtcNow:T} {nameof(MoveToPosition)} Camera Head {Name} - Complete");
        }

        /// <summary>
        /// Calculate the direction of travel and determine the distance traveled in degrees per interval at the given velocity.
        /// This gives the vector for movement to be added each interval to the current position
        /// </summary>
        /// <param name="current"></param>
        /// <param name="destination"></param>
        /// <param name="velocity">degrees / sec</param>
        /// <returns></returns>
        private double CalcVectorPerInterval(double current, double destination, double velocity) {
            var toTravel = destination - current;

            // Calc the distance we can travel each interval and multiple by the direction 
            return toTravel == 0.0 ? 0.0 : (destination > current ? 1 : -1)  * (velocity *(UpdateInterval/1000.00));
        }

        private bool MovementCompleted(Position destination) {
            // We can test for equality because the CalcNewPosition will stop exactly at destination 
            return _currentPosition.Equals(destination);
        }

        private async Task<Position> CalcNewPosition(double panVector, double tiltVector, Position destination) {
            RaiseOnStatusChanged(new StatusChangedEventArgs(CameraHeadStatus.Moving));

            await Task.Delay(UpdateInterval);

            var newPosition = new Position(
                IncrementByVector(_currentPosition.Pan, panVector, destination.Pan), 
                IncrementByVector(_currentPosition.Tilt, tiltVector, destination.Tilt));

            Debug.WriteLine($"{nameof(CalcNewPosition)} Camera Head {Name} - New {newPosition}");

            var panTimeToDestination = Math.Abs(destination.Pan - _currentPosition.Pan) / _panVelocity;
            var tiltTimeToDestination = Math.Abs(destination.Tilt - _currentPosition.Tilt) / _tiltVelocity;

            // Determine if Pan or Tilt movement wil take the longest.
            var timeToDestination = panTimeToDestination > tiltTimeToDestination
                ? panTimeToDestination
                : tiltTimeToDestination;

            RaiseOnPositionChanged(new CameraHeadPositionEventArgs(newPosition, TimeSpan.FromSeconds(timeToDestination)));

            return newPosition;
        }

        private double IncrementByVector(double currentPosition, double vector, double destination) {
            var newPosition = currentPosition + vector;
            if (vector > 0) {
                return newPosition >= destination ? destination : newPosition;
            }
            return newPosition <= destination ? destination : newPosition;
        }

        private void RaiseOnStatusChanged(StatusChangedEventArgs e) {
            Debug.WriteLine($"{DateTime.UtcNow:T} {nameof(RaiseOnStatusChanged)} Camera Head {Name} - Status {e.Status}");

            OnStatusChanged?.Invoke(this, e);
        }

        private void RaiseOnPositionChanged(CameraHeadPositionEventArgs e) {
            OnPositionChanged?.Invoke(this, e);
        }
    }
}