namespace Vitec.CameraHead.MotionTest {
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Vitec.CameraHead.MotionTest.Annotations;

    /// <summary>
    ///     Base ViewModel implementing <see cref="INotifyPropertyChanged" />
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}