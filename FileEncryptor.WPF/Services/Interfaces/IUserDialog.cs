using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace FileEncryptor.WPF.Services.Interfaces
{
    internal interface IUserDialog
    {
        bool OpenFile(string Title, out string SelectedFile, string Filter = "All (*.*)|*.*");
        bool OpenFiles(string Title, out IEnumerable<string> SelectedFiles, string Filter = "All (*.*)|*.*");

        bool SaveFile(string Title, out string SelectedFile, string DefaultFileName = null, string Filter = "All (*.*)|*.*");

        void Information(string Title, string Message);
        void Warning(string Title, string Message);
        void Error(string Title, string Message);

        (IProgress<double> Progress, IProgress<string> Status, CancellationToken Cancel, Action Close) ShowProgress(string Title);

    }
}
