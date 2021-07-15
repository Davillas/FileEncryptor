using System;
using System.Collections.Generic;
using System.Text;
using FileEncryptor.WPF.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace FileEncryptor.WPF.ViewModels
{
    static class ViewModelsRegistrator
    {
        public static IServiceCollection AddViewModels(this IServiceCollection services) => services
            .AddSingleton<MainWindowViewModel>()
        ;
    }
}
