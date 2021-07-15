using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileEncryptor.WPF.Services.Interfaces;
using Microsoft.Win32;

namespace FileEncryptor.WPF.Services
{
    class UserDialogService : IUserDialog
    {
        public bool OpenFile(string Title, out string SelectedFile, string Filter = "All (*.*)|*.*")
        {
            var file_dialog = new OpenFileDialog
            {
                Title = Title,
                Filter = Filter
            };

            if (file_dialog.ShowDialog() != true)
            {
                SelectedFile = null;
                return false;
            }

            SelectedFile = file_dialog.FileName;
            return true;
        }

        public bool OpenFiles(string Title, out IEnumerable<string> SelectedFiles, string Filter = "All (*.*)|*.*")
        {
            var file_dialog = new OpenFileDialog
            {
                Title = Title,
                Filter = Filter
            };

            if (file_dialog.ShowDialog() != true)
            {
                SelectedFiles = Enumerable.Empty<string>();
                return false;
            }

            SelectedFiles = file_dialog.FileNames;
            return true;
        }
    }
}
