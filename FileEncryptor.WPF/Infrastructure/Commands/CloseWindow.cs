using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using FileEncryptor.WPF.Infrastructure.Commands.Base;

namespace FileEncryptor.WPF.Infrastructure.Commands
{
    internal class CloseWindow :BaseCommand
    {
        protected override bool CanExecute(object parameter) =>
            (parameter as Window ?? App.FocusedWindow ?? App.ActiveWindow) != null;

        protected override void Execute(object parameter) =>
            (parameter as Window ?? App.FocusedWindow ?? App.ActiveWindow)?.Close();
    }
}
