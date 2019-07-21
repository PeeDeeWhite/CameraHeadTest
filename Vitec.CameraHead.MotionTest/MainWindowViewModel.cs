namespace Vitec.CameraHead.MotionTest {
    using System.Reflection;
    using Vitec.CameraHead.Models;
    using Vitec.CameraHead.MotionTest.Annotations;

    [UsedImplicitly]
    public class MainWindowViewModel : ViewModelBase {

        private Studio _studio;

        public MainWindowViewModel() {
            Studio = Configuration.BuildConfiguration(Assembly.GetExecutingAssembly()).CreateStudio();
        }

        public Studio Studio {
            get => _studio;
            set {
                if (Equals(value, _studio)) return;

                _studio = value;
                OnPropertyChanged();
            }
        }
    }
}