using MathCore.WPF.WindowTest.Services;
using MathCore.WPF.WindowTest.Services.Hosted;
using MathCore.WPF.WindowTest.Services.Interfaces;
using MathCore.WPF.WindowTest.ViewModels;

using Microsoft.Extensions.DependencyInjection;

namespace MathCore.WPF.WindowTest;

public partial class App
{
    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<TestWindow2ViewModel>();
        services.AddScoped<TestWindow3ViewModel>();
        services.AddTransient<IUserDialog, TestUserDialogService>();
        services.AddHostedService<MonitoringService>();
    }
}