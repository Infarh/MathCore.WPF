using MathCore.WPF.WindowTest.ViewModels;

using Microsoft.Extensions.DependencyInjection;

namespace MathCore.WPF.WindowTest
{
    internal class ServiceLocator
    {
        public TestWindow2ViewModel Model2 => App.Services.GetRequiredService<TestWindow2ViewModel>();

        public TestWindow3ViewModel Model3 => App.Services.GetRequiredService<TestWindow3ViewModel>();
    }
}
