using System.Windows;

namespace MathCore.WPF.Commands;

public class TestCommand : LambdaCommand
{
    public TestCommand() : base(p => MessageBox.Show(p?.ToString() ?? "null", "TestCommand")) { }
}