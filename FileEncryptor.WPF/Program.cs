using System;
using System.Collections.Generic;
using System.Text;

namespace FileEncryptor.WPF
{
    internal static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var app = new App();
            app.InitializeComponent();
            app.Run();

        }
    }
}
