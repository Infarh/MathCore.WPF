using System;
using Microsoft.Extensions.DependencyInjection;

namespace MathCore.WPF.WindowTest
{
    internal static class Program
    {
        [STAThread]
        public static void Main(string[] Args)
        {
            var app = new App();
            app.InitializeComponent();
            app.Run();
        }
    }

    public interface IWpfAppBuilder
    {

    }

    public class WpfAppBuilder
    {

    }

    public interface IWpfStartup
    {
        void ConfigureServices(IServiceCollection Services);
    }
}
