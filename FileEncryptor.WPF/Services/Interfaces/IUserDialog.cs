using System;
using System.Collections.Generic;
using System.Text;

namespace FileEncryptor.WPF.Services.Interfaces
{
    internal interface IUserDialog
    {
        bool OpenFile(string Title, out string SelectedFile, string Filter = "All (*.*)|*.*");
        bool OpenFiles(string Title, out IEnumerable<string> SelectedFiles, string Filter = "All (*.*)|*.*");
    }
}
