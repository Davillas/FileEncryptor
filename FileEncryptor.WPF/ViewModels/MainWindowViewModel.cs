using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Input;
using FileEncryptor.WPF.Infrastructure.Commands;
using FileEncryptor.WPF.Services.Interfaces;
using FileEncryptor.WPF.ViewModels.Base;

namespace FileEncryptor.WPF.ViewModels
{
    internal class MainWindowViewModel : BaseViewModel
    {
        private readonly IUserDialog _UserDialog;

        #region Title : string - Title of Window

        /// <summary>Title of Window</summary>
        private string _Title = "Encryptor";

        /// <summary>Title of Window</summary>
        public string Title
        {
            get => _Title;
            set => Set(ref _Title, value);
        }

        #endregion

        #region Password : string - Password

        /// <summary>Password</summary>
        private string _Password;

        /// <summary>Password</summary>
        public string Password
        {
            get => _Password;
            set => Set(ref _Password, value);
        }

        #endregion

        #region SelectedFile : FileInfo - Selected File Property

        /// <summary>Selected File Property</summary>
        private FileInfo _SelectedFile;

        /// <summary>Selected File Property</summary>
        public FileInfo SelectedFile
        {
            get => _SelectedFile;
            set => Set(ref _SelectedFile, value);
        }

        #endregion

        #region Commands

        #region Command SelectFileCommand - SelectFile

        /// <summary>SelectFile</summary>
        private ICommand _SelectFileCommand;

        /// <summary>SelectFile</summary>
        public ICommand SelectFileCommand => _SelectFileCommand
            ??= new LambdaCommand(OnSelectFileCommandExecuted, CanSelectFileCommandExecute);

        /// <summary>Проверка возможности выполнения - SelectFile</summary>
        private bool CanSelectFileCommandExecute() => true;

        /// <summary>Логика выполнения - SelectFile</summary>
        private void OnSelectFileCommandExecuted()
        {
            if(!_UserDialog.OpenFile("Choose File for encryption", out var file_path)) return;
            var selected_file = new FileInfo(file_path);
            SelectedFile = selected_file.Exists ? selected_file : null;
        }

        #endregion

        #endregion


        public MainWindowViewModel(IUserDialog UserDialog)
        {
            _UserDialog = UserDialog;
        }
    }
}
