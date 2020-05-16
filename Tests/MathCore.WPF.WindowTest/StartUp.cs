using System;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MathCore.WPF.WindowTest
{
    public class StartUp : IWpfStartup
    {
        public IConfiguration Configuration { get; }

        public StartUp(IConfiguration Configuration) => this.Configuration = Configuration;

        public void ConfigureServices(IServiceCollection Services)
        {

        }

        public void Configure(Application app, IServiceProvider Services)
        {

        }
    }
}