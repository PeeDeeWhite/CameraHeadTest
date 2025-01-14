﻿namespace Vitec.CameraHead.MotionTest;

using System;
using System.Threading.Tasks;
using System.Windows;
using Models;

/// <summary>
///     View model for displaying current status and progress of a camera head
/// </summary>
public class CameraHeadMonitorViewModel : ViewModelBase
{
    private readonly bool _wrapInAsyncCall;
    private Position _cameraTarget;
    private Position _currentPosition;
    private double _progressValue;
    private CameraHeadStatus _status;
    private Position _targetPosition;
    private TimeSpan _timeToShot;
    private double _totalDistanceToTravel;

    public CameraHeadMonitorViewModel(ICameraHead cameraHead, bool wrapInAsyncCall)
    {
        _wrapInAsyncCall = wrapInAsyncCall;
        MoveCommand = new RelayCommand(ExecuteMove, CanMove);
        StopCommand = new RelayCommand(ExecuteStop, CanStop);
        InitialiseCameraHead(cameraHead);
    }

    public RelayCommand MoveCommand { get; }
    public RelayCommand StopCommand { get; }

    /// <summary>
    ///     Gets/Sets the Camera Head
    /// </summary>
    public ICameraHead CameraHead { get; private set; }

    /// <summary>
    ///     Gets/Sets the current position of the camera head
    /// </summary>
    public Position CurrentPosition
    {
        get => _currentPosition;
        set => SetProperty(ref _currentPosition, value);
    }

    /// <summary>
    ///     Gets/Sets the position the camera head needs to reach for the currently selected target
    /// </summary>
    public Position TargetPosition
    {
        get => _targetPosition;
        set
        {
            SetProperty(ref _targetPosition, value);
            MoveCommand.RaiseCanExecuteChanged();
        }
    }

    /// <summary>
    ///     Gets/Sets the current status of the camera Head
    /// </summary>
    public CameraHeadStatus Status
    {
        get => _status;
        set => SetProperty(ref _status, value);
    }

    /// <summary>
    ///     Gets/Sets the time remaining to make the shot
    /// </summary>
    public TimeSpan TimeToShot
    {
        get => _timeToShot;
        set => SetProperty(ref _timeToShot, value);
    }

    /// <summary>
    ///     Gets/sets the percentage complete of the movement to the target.
    /// </summary>
    public double ProgressValue
    {
        get => _progressValue;
        set => SetProperty(ref _progressValue, value);
    }

    private void InitialiseCameraHead(ICameraHead cameraHead)
    {
        CameraHead = cameraHead;
        if (cameraHead != null)
        {
            CurrentPosition = new Position(0, 0);
            Status = CameraHeadStatus.Idle;
            InitCounters();

            CameraHead.PositionChanged += (_, args) =>
            {
                TimeToShot = args.TimeToShot;
                CurrentPosition = args.CurrentPosition;

                if (Status == CameraHeadStatus.Moving) ProgressValue = CalcPercentageComplete();
            };
            CameraHead.StatusChanged += (_, args) =>
            {
                Status = args.Status;

                if (Status == CameraHeadStatus.Idle) InitCounters();
                Application.Current.Dispatcher.Invoke(() => StopCommand.RaiseCanExecuteChanged());
            };
        }

        OnPropertyChanged(nameof(CameraHead));
    }

    private void InitCounters()
    {
        ProgressValue = 0;
        TimeToShot = TimeSpan.FromSeconds(0);
    }

    private void ExecuteMove()
    {
        // Take a snapshot of target position to use for calculation in case it is updated
        // by user selecting another target.
        _cameraTarget = TargetPosition;
        _totalDistanceToTravel = CurrentPosition.DistanceTo(_cameraTarget);
        InitCounters();

        if (_wrapInAsyncCall)
        {
            Task.Run(() => { CameraHead.Move(_cameraTarget); }).ConfigureAwait(false);
            return;
        }

        CameraHead.Move(_cameraTarget);
    }

    private bool CanMove()
    {
        return TargetPosition != null;
    }

    private bool CanStop()
    {
        return Status == CameraHeadStatus.Moving;
    }

    private void ExecuteStop()
    {
        CameraHead.StopCamera();
    }

    private double CalcPercentageComplete()
    {
        // Calc total distance to travel minus distance left to travel to get distance traveled. Divide by total distance
        var distanceLeft = CurrentPosition.DistanceTo(_cameraTarget);
        return (_totalDistanceToTravel - distanceLeft) / _totalDistanceToTravel * 100.00;
    }
}