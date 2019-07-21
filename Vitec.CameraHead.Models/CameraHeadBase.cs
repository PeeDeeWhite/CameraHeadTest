// ReSharper disable CompareOfFloatsByEqualityOperator 
namespace Vitec.CameraHead.Models {
    using System;
    using System.Threading.Tasks;

    public abstract class CameraHeadBase : ICameraHead {
        private readonly double _panVelocity;
        private readonly double _tiltVelocity;
        private protected double MaxPanRange;
        private protected double MaxTiltRange;
        private protected double MinPanRange;
        private protected double MinTiltRange;
        private protected int UpdateInterval; // Update interval in milliseconds

        public event EventHandler<CameraHeadPositionEventArgs> OnPositionChanged;

        public event EventHandler<StatusChangedEventArgs> OnStatusChanged;

        protected CameraHeadBase(string name, double panVelocity, double tiltVelocity) {
            Name = name;

            // Assumes pan / tilt velocity not changed during life-cycle of Camera Head    
            _panVelocity = panVelocity;
            _tiltVelocity = tiltVelocity;

            // Assume Head initialized to home location
            CurrentPosition = new Position(0.0, 0.0);
        }

        private Position CurrentPosition { get; set; }

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
            // Register head stopping before moving to new position
            RaiseOnStatusChanged(new StatusChangedEventArgs(CameraHeadStatus.Idle));

            if (CurrentPosition.Equals(position)) {
                return;
            }

            await MoveToPosition(CurrentPosition, AdjustForMinMaxRange(position), _panVelocity, _tiltVelocity).ConfigureAwait(false);
        }

        /// <summary>
        ///     Limit destination position to maximum range of head
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private Position AdjustForMinMaxRange(Position position) {
            var destinationPan = Math.Min(position.Pan, position.Pan > CurrentPosition.Pan ? MaxPanRange : MinPanRange);
            var destinationTilt = Math.Min(position.Tilt, position.Tilt > CurrentPosition.Tilt ? MaxTiltRange : MinTiltRange);

            return new Position(destinationPan, destinationTilt);
        }

        /// <summary>
        ///     Simulate moving to new position using the pan and tilt velocity
        ///     Raise events every UpdateInterval
        /// </summary>
        /// <param name="current"></param>
        /// <param name="destination"></param>
        /// <param name="panVelocity"></param>
        /// <param name="tiltVelocity"></param>
        /// <returns></returns>
        private async Task MoveToPosition(Position current, Position destination, double panVelocity, double tiltVelocity) {

            var panVector = CalcVectorPerInterval(current.Pan, destination.Pan, panVelocity);
            var tiltVector = CalcVectorPerInterval(current.Pan, destination.Pan, tiltVelocity);

            while (!MovementCompleted(destination)) {
                CurrentPosition = await CalcNewPosition(panVector, tiltVector, UpdateInterval);
            }
        }

        private double CalcVectorPerInterval(double current, double destination, double panVelocity) {
            var toTravel = current - destination;
            var timeToDestination = Math.Abs(current - destination) / panVelocity;

            return toTravel == 0.0 ? 0.0 : toTravel / timeToDestination;
        }

        private bool MovementCompleted(Position destination) {
            // Invoke custom equals 
            return CurrentPosition.Equals(destination);
        }

        private async Task<Position> CalcNewPosition(double panVector, double tiltVector, int interval) {
            RaiseOnStatusChanged(new StatusChangedEventArgs(CameraHeadStatus.Moving));
            await Task.Delay(interval);
            var newPosition = new Position(CurrentPosition.Pan + panVector, CurrentPosition.Tilt + tiltVector);
            RaiseOnPositionChanged(new CameraHeadPositionEventArgs(newPosition, TimeSpan.FromMilliseconds(Math.Max(panVector, tiltVector) * interval)));

            return newPosition;
        }

        protected virtual void RaiseOnStatusChanged(StatusChangedEventArgs e) {
            OnStatusChanged?.Invoke(this, e);
        }

        protected virtual void RaiseOnPositionChanged(CameraHeadPositionEventArgs e) {
            OnPositionChanged?.Invoke(this, e);
        }
    }
}