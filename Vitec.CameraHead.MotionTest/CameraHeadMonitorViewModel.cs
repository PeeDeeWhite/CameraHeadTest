namespace Vitec.CameraHead.MotionTest {
    using System;
    using Vitec.CameraHead.Models;

    /// <summary>
    /// View model for displaying current status and progress of a camera head
    /// </summary>
    public class CameraHeadMonitorViewModel : ViewModelBase {

        private ICameraHead _cameraHead;
        private double _totalDistanceToTravel;
        private Position _currentPosition;
        private Position _targetPosition;
        private CameraHeadStatus _status;
        private TimeSpan _timeToShot;
        private double _progressValue;

        public CameraHeadMonitorViewModel(ICameraHead cameraHead) {
            SetPositionCommand = new RelayCommand(OnSetPosition, CanSetPosition);
            InitialiseCameraHead(cameraHead);
        }

        private void InitialiseCameraHead(ICameraHead cameraHead) {
            _cameraHead = cameraHead;
            if (cameraHead != null)
            {
                CurrentPosition = new Position(0, 0);
                Status = CameraHeadStatus.Idle;
                _cameraHead.OnPositionChanged += (sender, args) => {
                    TimeToShot = args.TimeToShot;
                    CurrentPosition = args.CurrentPosition;
                };
                _cameraHead.OnStatusChanged += (sender, args) => {
                    Status = args.Status;

                    switch (Status) {

                        case CameraHeadStatus.Idle:
                        case CameraHeadStatus.Disabled:
                            ProgressValue = 0;
                            break;
                        case CameraHeadStatus.Moving:
                            ProgressValue = CalcPercentageComplete();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                };
            }
            OnPropertyChanged(nameof(CameraHead));
        }

        private void OnSetPosition() {
            _totalDistanceToTravel = DistanceToTravel(CurrentPosition, TargetPosition);
            ProgressValue = 0;
            CameraHead.SetPosition(TargetPosition);
        }

        private bool CanSetPosition()
        {
            return TargetPosition != null;
        }

        public RelayCommand SetPositionCommand { get; }

        /// <summary>
        /// Gets/Sets the Camera Head 
        /// </summary>
        public ICameraHead CameraHead {
            get => _cameraHead;
        }

        /// <summary>
        /// Gets/Sets the current position of the camera head
        /// </summary>
        public Position CurrentPosition {
            get => _currentPosition;
            set => SetProperty(ref _currentPosition, value);
        }

        /// <summary>
        /// Gets/Sets the position the camera head needs to reach for the currently selected target
        /// </summary>
        public Position TargetPosition {
            get => _targetPosition;
            set {
                SetProperty(ref _targetPosition, value);
                SetPositionCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Gets/Sets the current status of the camera Head
        /// </summary>
        public CameraHeadStatus Status {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        /// <summary>
        /// Gets/Sets the time remaining to make the shot
        /// </summary>
        public TimeSpan TimeToShot {
            get => _timeToShot;
            set => SetProperty(ref _timeToShot, value);
        }


        /// <summary>
        /// Gets/sets the percentage complete of the movement to the target.
        /// </summary>
        public double ProgressValue {
            get => _progressValue;
            set => SetProperty(ref _progressValue, value);
        }

        private double DistanceToTravel(Position startPosition, Position endPosition) {
            return Math.Abs(endPosition.Pan - startPosition.Pan) + Math.Abs(endPosition.Tilt - startPosition.Tilt);
        }

        private double CalcPercentageComplete() {
            // Calc total distance to travel minus distance left to travel over total distance
            return (_totalDistanceToTravel - DistanceToTravel(CurrentPosition, TargetPosition)) / _totalDistanceToTravel * 100.00;
        }
    }
}