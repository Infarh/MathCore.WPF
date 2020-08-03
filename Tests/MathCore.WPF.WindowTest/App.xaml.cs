using System;
using System.Collections.Generic;
using System.Windows;

using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

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
