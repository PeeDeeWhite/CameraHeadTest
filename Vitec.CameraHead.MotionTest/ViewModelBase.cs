﻿namespace Vitec.CameraHead.MotionTest;

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

/// <summary>
///     Base ViewModel implementing <see cref="INotifyPropertyChanged" />
/// </summary>
public abstract class ViewModelBase : DependencyObject, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected void SetProperty<T>(ref T member, T value, [CallerMemberName] string propertyName = null)
    {
        if (Equals(member, value)) return;

        member = value;
        OnPropertyChanged(propertyName);
    }
}