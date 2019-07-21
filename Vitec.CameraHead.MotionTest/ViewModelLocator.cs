namespace Vitec.CameraHead.MotionTest {
    using System;
    using System.ComponentModel;
    using System.Windows;

    public static class ViewModelLocator {

        public static bool GetAutoWireViewModel(DependencyObject obj) {
            return (bool) obj.GetValue(AutoWireViewModelProperty);
        }

        public static void SetAutoWireViewModel(DependencyObject obj, bool value) {
            obj.SetValue(AutoWireViewModelProperty, value);
        }

        public static readonly DependencyProperty AutoWireViewModelProperty =
            DependencyProperty.RegisterAttached("AutoWireViewModel",
                typeof(bool), typeof(ViewModelLocator),
                new PropertyMetadata(false, AutoWireViewModelChanged));

        private static void AutoWireViewModelChanged(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs e) {
            if (DesignerProperties.GetIsInDesignMode(dependencyObject)) return;

            var viewModelTypeName = dependencyObject.GetType() + "Model";
            var viewModelType = Type.GetType(viewModelTypeName);

            if (viewModelType == null) {
                throw new InvalidOperationException($"Invalid View model type {viewModelTypeName}");
            }

            ((FrameworkElement) dependencyObject).DataContext = Activator.CreateInstance(viewModelType);
        }
    }
}