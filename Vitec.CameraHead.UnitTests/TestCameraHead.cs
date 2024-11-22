// ReSharper disable CompareOfFloatsByEqualityOperator  Zero positions are valid

namespace Vitec.CameraHead.UnitTests;

using System;
using System.Threading.Tasks;
using Models;
using ITimer = System.Threading.ITimer;

/// <summary>
///     Sample Robotic Head
///     Decompiled from Interview1 assembly and added to this solution's name space
///     Updated to use fake timer for system tests
/// </summary>
public class TestCameraHead : ICameraHead, IDisposable, IAsyncDisposable
{
    private const int UpdateInterval = 100;
    private readonly object _myLock = new();
    private readonly double _panVelocity;
    private readonly double _tiltVelocity;
    private Position _currentPosition = new(0.0, 0.0);
    private Position _movePosition;
    private double _panVector;
    private CameraHeadStatus _status = CameraHeadStatus.Idle;
    private double _tiltVector;
    private TimeSpan _timeToShot;
    private readonly ITimer _timer;

    public TestCameraHead(string name, double panVelocity, double tiltVelocity, TimeProvider timeProvider)
    {
        _panVelocity = panVelocity;
        _tiltVelocity = tiltVelocity;
        _timer = timeProvider.CreateTimer(UpdateTimerElapsed, this, TimeSpan.FromSeconds(0), TimeSpan.FromMilliseconds(UpdateInterval));
        Name = name;
    }

    public string Name { get; }

    public void StopCamera()
    {
        SetStatus(CameraHeadStatus.Idle);
        DebugHelpers.Log($"Camera Head {Name} - Stopped @ {_currentPosition}");
    }

    public event EventHandler<CameraHeadPositionEventArgs> PositionChanged;

    public event EventHandler<StatusChangedEventArgs> StatusChanged;

    public void Move(Position position)
    {
        lock (_myLock)
        {
            _movePosition = position;
            var pan = position.Pan - _currentPosition.Pan;
            var tilt = position.Tilt - _currentPosition.Tilt;
            var maxTime = Math.Max(Math.Abs(pan) / _panVelocity, Math.Abs(tilt) / _tiltVelocity);
            _timeToShot = TimeSpan.FromMilliseconds(maxTime * UpdateInterval);
            _panVector = pan == 0.0 ? 0.0 : pan / maxTime;
            _tiltVector = tilt == 0.0 ? 0.0 : tilt / maxTime;
            DebugHelpers.Log($"Camera Head {Name} - Current {_currentPosition} to Target {position} - Pan Vector {_panVector:N2} Tilt Vector {_tiltVector:N2} Interval {UpdateInterval}");
        }
    }

    private void UpdateTimerElapsed(object state)
    {
        lock (_myLock)
        {
            if (_movePosition != null)
            {
                _movePosition = null;

                switch (_status)
                {
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

            DebugHelpers.Log($"Camera Head {Name} - Current {_currentPosition} TimeToShot {_timeToShot}");

            OnPositionChanged();

            if (_timeToShot.TotalMilliseconds == 0.0)
            {
                SetStatus(CameraHeadStatus.Idle);
                DebugHelpers.Log($"Camera Head {Name} - Complete");
            }
        }
    }

    private void SetStatus(CameraHeadStatus cameraHeadStatus)
    {
        _status = cameraHeadStatus;
        OnStatusChanged();
    }

    private void OnStatusChanged()
    {
        StatusChanged?.Invoke(this, new StatusChangedEventArgs(_status));
    }

    private void OnPositionChanged()
    {
        PositionChanged?.Invoke(this, new CameraHeadPositionEventArgs(_currentPosition, _timeToShot));
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (_timer != null) await _timer.DisposeAsync();
    }
}