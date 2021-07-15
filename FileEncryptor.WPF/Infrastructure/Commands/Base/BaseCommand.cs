using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace FileEncryptor.WPF.Infrastructure.Commands.Base
{
    abstract class BaseCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        private bool _Executable = true;

        public bool Executable
        {
            get => _Executable;
            set
            {
                if (_Executable == value) return;
                _Executable = value;
                CommandManager.InvalidateRequerySuggested();
            }
        }

        bool ICommand.CanExecute(object parameter) => _Executable && CanExecute(parameter);

        void ICommand.Execute(object parameter)
        {
            if(CanExecute(parameter))
                Execute(parameter);
        }

        protected virtual bool CanExecute(object parameter) => true;

        protected abstract void Execute(object parameter);

    }
}
