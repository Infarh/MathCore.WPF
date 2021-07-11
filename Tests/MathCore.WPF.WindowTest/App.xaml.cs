using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

using Microsoft.Extensions.Hosting;

namespace MathCore.WPF.WindowTest
{
    public partial class App
    {
        private static IHost? __Host;

        public static IHost Host => __Host ??= Microsoft.Extensions.Hosting.Host
           .CreateDefaultBuilder(Environment.GetCommandLineArgs())
           .ConfigureServices(ConfigureServices)
           .Build();

        public static IServiceProvider Services => Host.Services;

        private GlobalHotKey? _HotKey;

        protected override async void OnStartup(StartupEventArgs e)
        {
            var host = Host;
            base.OnStartup(e);
            await host.StartAsync();

            Activated += OnWindowActivated;
        }

        private void OnWindowActivated(object? Sender, EventArgs E)
        {
            if (Current.MainWindow is not { } window) return;
            Activated -= OnWindowActivated;

            var hwnd = window.GetWindowHandle();
            _HotKey = new GlobalHotKey(hwnd, Keys.F6);
            _HotKey.Pressed += (s, _) =>
            {
                var hk = (GlobalHotKey)s;
                Debug.WriteLine((hk.Modifiers == ModifierKeys.None ? null : $"{hk.Modifiers}+") + hk.Key);
            };
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            using var host = Host;
            await host.StopAsync();
            _HotKey?.Dispose();
        }
    }
}
