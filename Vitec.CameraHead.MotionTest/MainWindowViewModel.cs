namespace Vitec.CameraHead.MotionTest {
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Input;
    using Vitec.CameraHead.Models;
    using Vitec.CameraHead.MotionTest.Annotations;

    [UsedImplicitly]
    public class MainWindowViewModel : ViewModelBase {

        private Studio _studio;
        private Target _selectedTarget;

        private ObservableCollection<CameraHeadMonitorViewModel> _cameraHeadViewModels;

        public MainWindowViewModel() {
            var configuration = Configuration.BuildConfiguration(Assembly.GetExecutingAssembly());
            Studio = configuration.CreateStudio();

            var cameraHeads = configuration.GetConfigurationSection<IEnumerable<Models.Configuration.CameraHead>>(Constants.Configuration.CameraHeads).ToList();
            var childVMs = new ObservableCollection<CameraHeadMonitorViewModel>();
            foreach (var cameraHead in Studio.CameraHeads) {
                var wrapInAsyncCall = cameraHeads.First(x => x.Name == cameraHead.Name).WrapInAsyncCall;
                childVMs.Add(new CameraHeadMonitorViewModel(cameraHead, wrapInAsyncCall));
            }

            CameraHeadViewModels = childVMs;

            SetAllPositionsCommand = new RelayCommand(OnSetAllPositions, CanSetAllPositions);
        }

        public RelayCommand SetAllPositionsCommand { get; }

        private void OnSetAllPositions()
        {
            foreach (var cameraHeadMonitorViewModel in _cameraHeadViewModels) {
                ((ICommand)cameraHeadMonitorViewModel.SetPositionCommand).Execute(null);
            }
        }

        private bool CanSetAllPositions()
        {
            return SelectedTarget != null;
        }



        public Studio Studio {
            get => _studio;
            set => SetProperty(ref _studio, value);
        }

        public ObservableCollection<CameraHeadMonitorViewModel> CameraHeadViewModels {
            get => _cameraHeadViewModels;
            set => SetProperty(ref _cameraHeadViewModels, value);
        }

        public Target SelectedTarget {
            get => _selectedTarget;
            set {
                SetProperty(ref _selectedTarget, value);
                foreach (var viewModel in CameraHeadViewModels)
                {
                    viewModel.TargetPosition = value.Shots.First(x => x.HeadId == viewModel.CameraHead.Name).Position;
                }
                SetAllPositionsCommand.RaiseCanExecuteChanged();
            }
        }
    }
}