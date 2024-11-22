namespace Vitec.CameraHead.MotionTest;

using System;
using System.ComponentModel;
using System.Windows;

/// <summary>
///     Simple ViewModel locator that checks for a model with the same name
///     as the view suffixed by 'Model' and driven by the Attached property AutoWireViewModel
/// </summary>
public static class ViewModelLocator
{
    public static readonly DependencyProperty AutoWireViewModelProperty =
        DependencyProperty.RegisterAttached("AutoWireViewModel",
            typeof(bool), typeof(ViewModelLocator),
            new PropertyMetadata(false, AutoWireViewModelChanged));

    public static bool GetAutoWireViewModel(DependencyObject obj)
    {
        return (bool) obj.GetValue(AutoWireViewModelProperty);
    }

    public static void SetAutoWireViewModel(DependencyObject obj, bool value)
    {
        obj.SetValue(AutoWireViewModelProperty, value);
    }

    private static void AutoWireViewModelChanged(DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs e)
    {
        if (DesignerProperties.GetIsInDesignMode(dependencyObject)) return;

        var viewModelTypeName = dependencyObject.GetType() + "Model";
        var viewModelType = Type.GetType(viewModelTypeName);

        if (viewModelType == null) throw new InvalidOperationException(string.Format(Constants.ErrorMessages.InvalidViewModelType, viewModelTypeName));

        ((FrameworkElement) dependencyObject).DataContext = Activator.CreateInstance(viewModelType);
    }
}