using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Interop;

using MathCore.WPF.WindowTest.ViewModels;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MathCore.WPF.WindowTest;

public partial class App
{
    private static IHost? __Host;

    public static IHost Host => __Host ??= Microsoft.Extensions.Hosting.Host
       .CreateDefaultBuilder(Environment.GetCommandLineArgs())
#if DEBUG
       .InitializeObject(host => host.UseEnvironment("Development"))
#endif
       .InitializeObject(host => host.ConfigureHostOptions(o => o.ServicesStartConcurrently = true))
       .ConfigureServices(ConfigureServices)
       .Build();

    public static IServiceProvider Services => Host.Services;

    protected override async void OnStartup(StartupEventArgs e)
    {
        ComponentDispatcher.ThreadPreprocessMessage += OnThreadPreprocessMessage;
        ComponentDispatcher.ThreadIdle += OnThreadIdle;

        var host = Host;

        //var console_logger = Services.GetRequiredService<ILogger<App>>();
        //var old_out = Console.Out;

        //Console.SetOut(new LambdaTextWriter(s =>
        //{
        //    console_logger.LogInformation(s.TrimEnd());
        //    //old_out.Write(s);
        //}));

        base.OnStartup(e);
        await host.StartAsync();

        var main_thread_cancellation = __MainThreadWatcherCancellation.Token;
        _ = Task.Run(() => MainThreadLoadingWatcher(main_thread_cancellation), main_thread_cancellation);
    }

    private static readonly CancellationTokenSource __MainThreadWatcherCancellation = new();
    private static async Task MainThreadLoadingWatcher(CancellationToken cancel)
    {
        while (true)
        {
            Debug.WriteLine("Thread id:0 loading time {0}", TimeSpan.FromMicroseconds(MainThreadLoadingTimeInMilliseconds));
            await Task.Delay(500, cancel).ConfigureAwait(false);
        }
    }

    private static readonly Stopwatch __MainThreadTimer = Stopwatch.StartNew();

    public static long MainThreadLoadingTimeInMilliseconds => __MainThreadTimer.ElapsedMilliseconds;

    private static void OnThreadPreprocessMessage(ref MSG Msg, ref bool Handled) => __MainThreadTimer.Restart();

    private static void OnThreadIdle(object? Sender, EventArgs E) => __MainThreadTimer.Stop();


    protected override async void OnExit(ExitEventArgs e)
    {
        await __MainThreadWatcherCancellation.CancelAsync();
        __MainThreadWatcherCancellation.Dispose();
        base.OnExit(e);
        using var host = Host;
        await host.StopAsync();

        Resources.Values.OfType<IDisposable>().Foreach(v => v.Dispose());
    }
}