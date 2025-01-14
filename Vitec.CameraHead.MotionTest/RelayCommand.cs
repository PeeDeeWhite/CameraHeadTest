﻿// ReSharper disable ConstantConditionalAccessQualifier -- Conditional call to Invoke incorrectly flagged as an issue

namespace Vitec.CameraHead.MotionTest;

using System;
using System.Windows.Input;

/// <summary>
///     Simple implementation of <see cref="ICommand" /> with RaiseCanExecuteChanged function
/// </summary>
public class RelayCommand : ICommand
{
    private readonly Func<bool> _targetCanExecuteMethod;
    private readonly Action _targetExecuteMethod;

    public RelayCommand(Action executeMethod)
    {
        _targetExecuteMethod = executeMethod;
    }

    public RelayCommand(Action executeMethod, Func<bool> canExecuteMethod)
    {
        _targetExecuteMethod = executeMethod;
        _targetCanExecuteMethod = canExecuteMethod;
    }

    public event EventHandler CanExecuteChanged;

    bool ICommand.CanExecute(object parameter)
    {
        if (_targetCanExecuteMethod != null) return _targetCanExecuteMethod();

        return _targetExecuteMethod != null;
    }

    void ICommand.Execute(object parameter)
    {
        _targetExecuteMethod?.Invoke();
    }

    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}