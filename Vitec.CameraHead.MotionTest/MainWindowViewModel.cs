namespace Vitec.CameraHead.MotionTest {
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using Vitec.CameraHead.Models;
    using Vitec.CameraHead.MotionTest.Annotations;

    [UsedImplicitly]
    public class MainWindowViewModel : ViewModelBase {

        private Studio _studio;
        private Target _selectedTarget;

        private ObservableCollection<CameraHeadMonitorViewModel> _cameraHeadViewModels;

        public MainWindowViewModel() {
            Studio = Configuration.BuildConfiguration(Assembly.GetExecutingAssembly()).CreateStudio();
            
            var childVMs = new ObservableCollection<CameraHeadMonitorViewModel>();
            foreach (var cameraHead in Studio.CameraHeads) {
                childVMs.Add(new CameraHeadMonitorViewModel(cameraHead));
            }

            CameraHeadViewModels = childVMs;
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
            }
        }
    }
}