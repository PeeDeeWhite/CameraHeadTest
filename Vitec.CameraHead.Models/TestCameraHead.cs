// ReSharper disable CompareOfFloatsByEqualityOperator  Zero positions are valid
namespace Vitec.CameraHead.Models {
    using System;
    using System.Diagnostics;
    using System.Timers;

    /// <summary>
    /// Sample Robotic Head
    /// Uses timer to simulate movement
    /// Decompiled from Interview1 assembly and added to this solutions name space
    /// </summary>
    public class TestCameraHead : ICameraHead {
        private readonly double _panVelocity;
        private readonly double _tiltVelocity;
        private const double PanVelocity = 0.50123;
        private const double TiltVelocity = 0.50123;
        private const int UpdateInterval = 100;
        private Position _currentPosition = new Position(0.0, 0.0);
        private bool _finished;
        private Position _movePosition;
        private readonly object _myLock = new object();
        private double _panVector;
        private CameraHeadStatus _status = CameraHeadStatus.Idle;
        private double _tiltVector;
        private TimeSpan _timeToShot;
        private readonly Timer _updateTimer = new Timer(UpdateInterval);

        public TestCameraHead(string name, double panVelocity = PanVelocity, double tiltVelocity = TiltVelocity) {
            _panVelocity = panVelocity;
            _tiltVelocity = tiltVelocity;
            Name = name;
            _updateTimer.AutoReset = true;
            _updateTimer.Elapsed += UpdateTimerElapsed;
            _updateTimer.Start();
        }

        public string Name { get; }

        public event EventHandler<CameraHeadPositionEventArgs> OnPositionChanged;

        public event EventHandler<StatusChangedEventArgs> OnStatusChanged;

        public void SetPosition(Position position) {
            lock (_myLock) {
                _movePosition = position;
                var num1 = position.Pan - _currentPosition.Pan;
                var num2 = position.Tilt - _currentPosition.Tilt;
                var num3 = Math.Max(Math.Abs(num1) / _panVelocity, Math.Abs(num2) / _tiltVelocity);
                _timeToShot = TimeSpan.FromMilliseconds(num3 * UpdateInterval);
                _panVector = num1 == 0.0 ? 0.0 : num1 / num3;
                _tiltVector = num2 == 0.0 ? 0.0 : num2 / num3;
                Debug.WriteLine($"{DateTime.UtcNow:T} {nameof(SetPosition)} Camera Head {Name} - Current {_currentPosition} to Target {position} - Pan Vector {_panVector:N2} Tilt Vector {_tiltVector:N2} Interval {UpdateInterval}");
                _finished = false;
            }

            do { } while (!_finished);
            Debug.WriteLine($"{DateTime.UtcNow:T} {nameof(SetPosition)} Camera Head {Name} - Complete");
        }

        private void UpdateTimerElapsed(object sender, ElapsedEventArgs e) {
            lock (_myLock) {
                if (_movePosition != null) {
                    _movePosition = null;

                    switch (_status) {
                        case CameraHeadStatus.Idle:
                            SetStatus(CameraHeadStatus.Moving);

                            break;
                        case CameraHeadStatus.Moving:
                            SetStatus(CameraHeadStatus.Idle);
                            SetStatus(CameraHeadStatus.Moving);

                            break;
                    }
                }

                if (_status != CameraHeadStatus.Moving)
                    return;

                _currentPosition = new Position(_currentPosition.Pan + _panVector, _currentPosition.Tilt + _tiltVector);
                _timeToShot = _timeToShot.Subtract(TimeSpan.FromMilliseconds(UpdateInterval));
                if (_timeToShot.TotalMilliseconds < 0.0)
                    _timeToShot = TimeSpan.Zero;

                Debug.WriteLine($"{DateTime.UtcNow:T} {nameof(SetPosition)} Camera Head {Name} - Current {_currentPosition} TimeToShot {_timeToShot}");

                RaisePositionChanged();

                if (_timeToShot.TotalMilliseconds == 0.0) {
                    SetStatus(CameraHeadStatus.Idle);
                    _finished = true;
                }
            }
        }

        private void SetStatus(CameraHeadStatus cameraHeadStatus) {
            _status = cameraHeadStatus;
            RaiseStatusChanged();
        }

        private void RaiseStatusChanged() {
            if (OnStatusChanged == null)
                return;

            OnStatusChanged(this, new StatusChangedEventArgs(_status));
        }

        private void RaisePositionChanged() {
            if (OnPositionChanged == null)
                return;

            OnPositionChanged(this, new CameraHeadPositionEventArgs(_currentPosition, _timeToShot));
        }
    }
}