using System.Diagnostics;
using System.IO;
using System.Windows;

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
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);
        using var host = Host;
        await host.StopAsync();
    }
}