using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace MathCore.WPF.WindowTest
{
    public partial class App
    {
        public ServiceProvider Services { get; }

        public App()
        {
            var service_collection = new ServiceCollection();
            ConfigureServices(service_collection);
            Services = service_collection.BuildServiceProvider();
            Configure();
        }
    }
}
