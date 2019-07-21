﻿// ReSharper disable ConstantConditionalAccessQualifier -- Conditional call to Invoke incorrectly flagged as an issue
namespace Vitec.CameraHead.MotionTest
{
    using System;
    using System.Windows.Input;

    public class RelayCommand : ICommand
    {
        private readonly Action _targetExecuteMethod;
        private readonly Func<bool> _targetCanExecuteMethod;

        public RelayCommand(Action executeMethod)
        {
            _targetExecuteMethod = executeMethod;
        }

        public RelayCommand(Action executeMethod, Func<bool> canExecuteMethod)
        {
            _targetExecuteMethod = executeMethod;
            _targetCanExecuteMethod = canExecuteMethod;
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged(this, EventArgs.Empty);
        }

        bool ICommand.CanExecute(object parameter)
        {
            if (_targetCanExecuteMethod != null)
            {
                return _targetCanExecuteMethod();
            }

            return _targetExecuteMethod != null;
        }

        public event EventHandler CanExecuteChanged = delegate { };

        void ICommand.Execute(object parameter) {
            _targetExecuteMethod?.Invoke();
        }
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _targetExecuteMethod;
        private readonly Func<T, bool> _targetCanExecuteMethod;

        public RelayCommand(Action<T> executeMethod)
        {
            _targetExecuteMethod = executeMethod;
        }

        public RelayCommand(Action<T> executeMethod, Func<T,bool> canExecuteMethod)
        {
            _targetExecuteMethod = executeMethod;
            _targetCanExecuteMethod = canExecuteMethod;
        }

        public void RaiseCanExecuteChanged() 
        {
             CanExecuteChanged(this, EventArgs.Empty); 
        }

        bool ICommand.CanExecute(object parameter)
        {
            if (_targetCanExecuteMethod != null)
            {
                return _targetCanExecuteMethod((T)parameter);
            }

            return _targetExecuteMethod != null;
        }

        public event EventHandler CanExecuteChanged = delegate { };

        void ICommand.Execute(object parameter) {
            _targetExecuteMethod?.Invoke((T)parameter);
        }
    }
}