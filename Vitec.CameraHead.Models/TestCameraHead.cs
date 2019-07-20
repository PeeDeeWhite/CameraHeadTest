// ReSharper disable CompareOfFloatsByEqualityOperator  Zero positions are valid
namespace Vitec.CameraHead.Models {
    using System;
    using System.Timers;

    /// <summary>
    /// Sample Robotic Head
    /// Uses timer to simulate movement
    /// Decompiled from Interview1 assembly and added to this solutions name space
    /// </summary>
    public class TestCameraHead : ICameraHead {
        private const double PanVelocity = 0.50123;
        private const double TiltVelocity = 0.50123;
        private const int UpdateInterval = 100;
        private Position currentPosition = new Position(0.0, 0.0);
        private bool finished;
        private Position movePosition;
        private readonly object myLock = new object();
        private double panVector;
        private CameraHeadStatus status = CameraHeadStatus.Idle;
        private double tiltVector;
        private TimeSpan timeToShot;
        private readonly Timer updateTimer = new Timer(UpdateInterval);

        public TestCameraHead(string name) {
            Name = name;
            updateTimer.AutoReset = true;
            updateTimer.Elapsed += UpdateTimerElapsed;
            updateTimer.Start();
        }

        public string Name { get; }

        public event EventHandler<CameraHeadPositionEventArgs> OnPositionChanged;

        public event EventHandler<StatusChangedEventArgs> OnStatusChanged;

        public void SetPosition(Position position) {
            lock (myLock) {
                movePosition = position;
                var num1 = position.Pan - currentPosition.Pan;
                var num2 = position.Tilt - currentPosition.Tilt;
                var num3 = Math.Max(Math.Abs(num1) / PanVelocity, Math.Abs(num2) / TiltVelocity);
                timeToShot = TimeSpan.FromMilliseconds(num3 * UpdateInterval);
                panVector = num1 == 0.0 ? 0.0 : num1 / num3;
                tiltVector = num2 == 0.0 ? 0.0 : num2 / num3;
                finished = false;
            }

            do { } while (!finished);
        }

        private void UpdateTimerElapsed(object sender, ElapsedEventArgs e) {
            lock (myLock) {
                if (movePosition != null) {
                    movePosition = null;

                    switch (status) {
                        case CameraHeadStatus.Idle:
                            SetStatus(CameraHeadStatus.Moving);

                            break;
                        case CameraHeadStatus.Moving:
                            SetStatus(CameraHeadStatus.Idle);
                            SetStatus(CameraHeadStatus.Moving);

                            break;
                    }
                }

                if (status != CameraHeadStatus.Moving)
                    return;

                currentPosition = new Position(currentPosition.Pan + panVector, currentPosition.Tilt + tiltVector);
                timeToShot = timeToShot.Subtract(TimeSpan.FromMilliseconds(UpdateInterval));
                if (timeToShot.TotalMilliseconds < 0.0)
                    timeToShot = TimeSpan.Zero;
                RaisePositionChanged();

                if (timeToShot.TotalMilliseconds == 0.0) {
                    SetStatus(CameraHeadStatus.Idle);
                    finished = true;
                }
            }
        }

        private void SetStatus(CameraHeadStatus cameraHeadStatus) {
            status = cameraHeadStatus;
            RaiseStatusChanged();
        }

        private void RaiseStatusChanged() {
            if (OnStatusChanged == null)
                return;

            OnStatusChanged(this, new StatusChangedEventArgs(status));
        }

        private void RaisePositionChanged() {
            if (OnPositionChanged == null)
                return;

            OnPositionChanged(this, new CameraHeadPositionEventArgs(currentPosition, timeToShot));
        }
    }
}