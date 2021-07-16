using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private string __EncryptedFileSuffix = ".encrypted";

        private readonly IUserDialog _UserDialog;
        private readonly IEncryptor _Encryptor;

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
        private string _Password = "123";

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

        #region Command EncryptCommand - Encryption Command

        /// <summary>Encryption Command</summary>
        private ICommand _EncryptCommand;

        /// <summary>Encryption Command</summary>
        public ICommand EncryptCommand => _EncryptCommand
            ??= new LambdaCommand(OnEncryptCommandExecuted, CanEncryptCommandExecute);

        /// <summary>Проверка возможности выполнения - Encryption Command</summary>
        private bool CanEncryptCommandExecute(object p) => (p is FileInfo file && file.Exists || SelectedFile !=null) && !string.IsNullOrWhiteSpace(Password);

        /// <summary>Логика выполнения - Encryption Command</summary>
        private void OnEncryptCommandExecuted(object p)
        {
            var file = p as FileInfo ?? SelectedFile;
            if(file is null) return;

            var destination_file_name = file.FullName + __EncryptedFileSuffix;
            if(!_UserDialog.SaveFile("Choose save destination", out var destination_path, destination_file_name)) return;


            var timer = Stopwatch.StartNew();
            _Encryptor.Encrypt(file.FullName, destination_path, Password);
            timer.Stop();
            _UserDialog.Information("Encryption", $"Encryption has been finished in {timer.Elapsed.TotalSeconds:0.##}s");
        }

        #endregion

        #region Command DecryptCommand - Decryption Command

        /// <summary>Decryption Command</summary>
        private ICommand _DecryptCommand;

        /// <summary>Decryption Command</summary>
        public ICommand DecryptCommand => _DecryptCommand
            ??= new LambdaCommand(OnDecryptCommandExecuted, CanDecryptCommandExecute);

        /// <summary>Проверка возможности выполнения - Decryption Command</summary>
        private bool CanDecryptCommandExecute(object p) => (p is FileInfo file && file.Exists || SelectedFile != null) && !string.IsNullOrWhiteSpace(Password);

        /// <summary>Логика выполнения - Decryption Command</summary>
        private void OnDecryptCommandExecuted(object p)
        {
            var file = p as FileInfo ?? SelectedFile;
            if (file is null) return;

            var destination_file_name = file.FullName.EndsWith(__EncryptedFileSuffix)
                ? file.FullName.Substring(0, file.FullName.Length - __EncryptedFileSuffix.Length)
                : file.FullName;
            if (!_UserDialog.SaveFile("Choose save destination", out var destination_path, destination_file_name)) return;

            var timer = Stopwatch.StartNew();
            var success = _Encryptor.Decrypt(file.FullName, destination_path, Password);
            timer.Stop();

            if (success)
                _UserDialog.Information("Decryption", $"Decryption is successfully finished in {timer.Elapsed.TotalSeconds:0.##}s");
            else
                _UserDialog.Warning("Decryption", "Warning: Incorrect Password.");
        }

        #endregion

        #endregion


        public MainWindowViewModel(IUserDialog UserDialog, IEncryptor Encryptor)
        {
            _UserDialog = UserDialog;
            _Encryptor = Encryptor;
        }
    }
}
