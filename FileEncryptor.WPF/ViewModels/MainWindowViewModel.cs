using System;
using System.Collections.Generic;
using System.Text;
using FileEncryptor.WPF.ViewModels.Base;

namespace FileEncryptor.WPF.ViewModels
{
    internal class MainWindowViewModel : BaseViewModel
    {
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
    }
}
