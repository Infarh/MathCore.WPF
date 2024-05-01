using System.Windows.Controls;

namespace MathCore.WPF.WindowTest;

internal static class Program
{
    [STAThread]
    public static void Main(string[] Args)
    {
        var property = DependencyPropertyBuilder
           .Register<TestControl>()
           .Property(p => p.Title)
           .OnChanged((_, _, _) => { });

        var app = new App();
        app.InitializeComponent();
        app.Run();
    }
}
    
internal class TestControl : Control
{
    public string Title { get; set; }
}