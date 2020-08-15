using System;
using System.Net.Http;
using System.Threading.Tasks;

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
}
