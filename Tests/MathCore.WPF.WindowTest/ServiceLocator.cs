using MathCore.WPF.WindowTest.ViewModels;

using Microsoft.Extensions.DependencyInjection;

namespace MathCore.WPF.WindowTest;

internal class ServiceLocator : IDisposable
{
    private readonly IServiceScope _ServicesScope = App.Services.CreateScope();

    public TestWindow2ViewModel Model2 => _ServicesScope.ServiceProvider.GetRequiredService<TestWindow2ViewModel>();

    public TestWindow3ViewModel Model3 => _ServicesScope.ServiceProvider.GetRequiredService<TestWindow3ViewModel>();
    public TestWindow4ViewModel Model4 => _ServicesScope.ServiceProvider.GetRequiredService<TestWindow4ViewModel>();
    public TestWindow5ViewModel Model5 => _ServicesScope.ServiceProvider.GetRequiredService<TestWindow5ViewModel>();

    public void Dispose() => _ServicesScope.Dispose();
}