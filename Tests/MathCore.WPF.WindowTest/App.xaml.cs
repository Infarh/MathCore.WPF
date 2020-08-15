using System;
using System.Windows;
using Microsoft.Extensions.Hosting;

namespace MathCore.WPF.WindowTest
{
    public partial class App
    {
        private IHost? _Host;

        public IHost Host => _Host ??= Microsoft.Extensions.Hosting.Host
           .CreateDefaultBuilder(Environment.GetCommandLineArgs())
           .ConfigureServices(ConfigureServices)
           .Build();

        public IServiceProvider Services => Host.Services;

        protected override async void OnStartup(StartupEventArgs e)
        {
            var host = Host;
            base.OnStartup(e);
            await host.StartAsync();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            using var host = Host;
            await host.StopAsync();
        }
    }
}
