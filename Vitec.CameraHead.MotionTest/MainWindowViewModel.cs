namespace Vitec.CameraHead.MotionTest {
    using System.Collections.ObjectModel;
    using Vitec.CameraHead.Models;
    using Vitec.CameraHead.MotionTest.Annotations;

    [UsedImplicitly]
    public class MainWindowViewModel : ViewModelBase {

        private Studio _studio;
        private ObservableCollection<Target> _targets;

        public MainWindowViewModel() {
            Studio = new Studio();
            Targets = new ObservableCollection<Target>(Studio.Targets); 
        }

        public Studio Studio {
            get => _studio;
            set {
                if (Equals(value, _studio)) return;

                _studio = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Target> Targets {
            get => _targets;
            set {
                if (Equals(value, _targets)) return;

                _targets = value;
                OnPropertyChanged();
            }
        }
    }
}