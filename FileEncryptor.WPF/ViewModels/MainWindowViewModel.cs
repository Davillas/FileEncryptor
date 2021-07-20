using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Input;
using FileEncryptor.WPF.Infrastructure.Commands;
using FileEncryptor.WPF.Infrastructure.Commands.Base;
using FileEncryptor.WPF.Services.Interfaces;
using FileEncryptor.WPF.ViewModels.Base;

namespace FileEncryptor.WPF.ViewModels
{
    internal class MainWindowViewModel : BaseViewModel
    {
        private string __EncryptedFileSuffix = ".encrypted";

        private readonly IUserDialog _UserDialog;
        private readonly IEncryptor _Encryptor;

        private CancellationTokenSource _ProcessAbortion;

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

        #region ProgressValue : double - ProgressValue

        /// <summary>ProgressValue</summary>
        private double _ProgressValue;

        /// <summary>ProgressValue</summary>
        public double ProgressValue
        {
            get => _ProgressValue;
            set => Set(ref _ProgressValue, value);
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
        private async void OnEncryptCommandExecuted(object p)
        {
            var file = p as FileInfo ?? SelectedFile;
            if(file is null) return;

            var destination_file_name = file.FullName + __EncryptedFileSuffix;
            if(!_UserDialog.SaveFile("Choose save destination", out var destination_path, destination_file_name)) return;


            var timer = Stopwatch.StartNew();

            var progress = new Progress<double>(percent => ProgressValue = percent);

            _ProcessAbortion = new CancellationTokenSource();
            var cancel = _ProcessAbortion.Token;

            var (progress_info, status_info, operation_cancel, close_window) = _UserDialog.ShowProgress("Encryption");
            status_info.Report($"File: {file.Name} Encryption");

            var combine_cancellation = CancellationTokenSource.CreateLinkedTokenSource(cancel, operation_cancel);


            ((BaseCommand) EncryptCommand).Executable = false;
            ((BaseCommand) DecryptCommand).Executable = false;
            
            /*   Additional code that runs in parallel to encryption process   */
            try
            {
                await _Encryptor.EncryptAsync(file.FullName, destination_path, Password, Progress: progress_info,
                    Cancel: combine_cancellation.Token);
            }
            catch (OperationCanceledException e) when(e.CancellationToken == combine_cancellation.Token)
            {
            }
            finally
            {
                _ProcessAbortion.Dispose();
                _ProcessAbortion = null;
            }
            ((BaseCommand)EncryptCommand).Executable = true;
            ((BaseCommand)DecryptCommand).Executable = true;

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
        private async void OnDecryptCommandExecuted(object p)
        {
            var file = p as FileInfo ?? SelectedFile;
            if (file is null) return;

            var destination_file_name = file.FullName.EndsWith(__EncryptedFileSuffix)
                ? file.FullName.Substring(0, file.FullName.Length - __EncryptedFileSuffix.Length)
                : file.FullName;
            if (!_UserDialog.SaveFile("Choose save destination", out var destination_path, destination_file_name)) return;

            var timer = Stopwatch.StartNew();


            var progress = new Progress<double>(percent => ProgressValue = percent);
            _ProcessAbortion = new CancellationTokenSource();
            var cancel = _ProcessAbortion.Token;

            var (progress_info, status_info, operation_cancel, close_window) = _UserDialog.ShowProgress("Encryption");
            status_info.Report($"File: {file.Name} Encryption");

            var combine_cancellation = CancellationTokenSource.CreateLinkedTokenSource(cancel, operation_cancel);

            ((BaseCommand)EncryptCommand).Executable = false;
            ((BaseCommand)DecryptCommand).Executable = false;
            var decryption_task = _Encryptor.DecryptAsync(file.FullName, destination_path, Password, Progress: progress, Cancel: _ProcessAbortion.Token);
            /*   Additional code that runs in parallel to encryption process   */
            var success = false;
            try
            {
                success = await decryption_task;
            }
            catch (OperationCanceledException e) when (e.CancellationToken == combine_cancellation.Token)
            {
            }
            finally
            {
                _ProcessAbortion.Dispose();
                _ProcessAbortion = null;
                close_window();
            }
            ((BaseCommand)EncryptCommand).Executable = true;
            ((BaseCommand)DecryptCommand).Executable = true;


            timer.Stop();

            if (success)
                _UserDialog.Information("Decryption", $"Decryption is successfully finished in {timer.Elapsed.TotalSeconds:0.##}s");
            else
                _UserDialog.Warning("Decryption", "Warning: Incorrect Password.");
        }

        #endregion

        #region Command AbortCommand - Abort Operation

        /// <summaryAbort Operation</summary>
        private ICommand _AbortCommand;

        /// <summary>Abort Operation</summary>
        public ICommand AbortCommand => _AbortCommand
            ??= new LambdaCommand(OnAbortCommandExecuted, CanAbortCommandExecute);

        /// <summary>Проверка возможности выполнения - Abort Operation</summary>
        private bool CanAbortCommandExecute(object p) => _ProcessAbortion != null && !_ProcessAbortion.IsCancellationRequested;

        /// <summary>Логика выполнения - Abort Operation</summary>
        private void OnAbortCommandExecuted(object p) => _ProcessAbortion.Cancel();

        #endregion
        #endregion


        public MainWindowViewModel(IUserDialog UserDialog, IEncryptor Encryptor)
        {
            _UserDialog = UserDialog;
            _Encryptor = Encryptor;
        }
    }
}
