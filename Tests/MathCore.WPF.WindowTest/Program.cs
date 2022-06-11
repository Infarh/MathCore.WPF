using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Controls;

using OxyPlot.Series;

namespace MathCore.WPF.WindowTest;

internal static class Program
{
    [STAThread]
    public static void Main(string[] Args)
    {
        var property = DependencyPropertyBuilder
           .Register<TestControl>()
           .Property(p => p.Title)
           .OnChanged((control, OldValue, NewValue) => { });

        var app = new App();
        app.InitializeComponent();
        app.Run();
    }
}

internal class TestControl : Control
{
    public string Title { get; set; }
}