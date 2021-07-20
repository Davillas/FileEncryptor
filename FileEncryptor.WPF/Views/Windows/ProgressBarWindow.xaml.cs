using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FileEncryptor.WPF.Views.Windows
{
    /// <summary>
    /// Interaction logic for ProgressBarWindow.xaml
    /// </summary>
    public partial class ProgressBarWindow : Window
    {

        #region Status : string - Status Message

        /// <summary>$summary$</summary>
        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register(
                nameof(Status),
                typeof(string),
                typeof(ProgressBarWindow),
                new PropertyMetadata(default(string)));

        /// <summary>$summary$</summary>
        //[Category("")]
        [Description("Status Message")]
        public string Status
        {
            get => (string) GetValue(StatusProperty);
            set => SetValue(StatusProperty, value);
        }

        #endregion

        #region ProgressValue : double - Progress Value

        /// <summary>Progress Value</summary>
        public static readonly DependencyProperty ProgressValueProperty =
            DependencyProperty.Register(
                nameof(ProgressValue),
                typeof(double),
                typeof(ProgressBarWindow),
                new PropertyMetadata(double.NaN, OnProgressChanged));

        private static void OnProgressChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var progress_value = (double) e.NewValue;
            var progress_view = ((ProgressBarWindow) d).ProgressView;
            progress_view.Value = progress_value;
            progress_view.IsIndeterminate = double.IsNaN(progress_value);
        }

        /// <summary>Progress Value</summary>
        //[Category("")]
        [Description("Progress Value")]
        public double ProgressValue
        {
            get => (double) GetValue(ProgressValueProperty);
            set => SetValue(ProgressValueProperty, value);
        }

        #endregion

        private IProgress<double> _ProgressInformer;
        public IProgress<double> ProgressInformer => _ProgressInformer ??= new Progress<double>(p=> ProgressValue = p);

        private IProgress<string> _StatusInformer;
        public IProgress<string> StatusInformer => _StatusInformer ??= new Progress<string>(status=> Status = status);

        
        private IProgress<(double Percent, string Message)> _ProgressStatusInformer;
        public IProgress<(double Percent, string Message)>  ProgressStatusInformer => 
            _ProgressStatusInformer ??= new Progress<(double Percent, string Message)>(
            p =>
            {
                ProgressValue = p.Percent;
                Status = p.Message;
            });


        private CancellationTokenSource _Cancellation;
        public CancellationToken Cancel
        {
            get
            {
                if (_Cancellation != null) return _Cancellation.Token;
                _Cancellation = new CancellationTokenSource();
                CancelButton.IsEnabled = true;
                return _Cancellation.Token;
            }
        }

        public ProgressBarWindow()
        {
            InitializeComponent();
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            _Cancellation?.Cancel();
        }
    }
}
