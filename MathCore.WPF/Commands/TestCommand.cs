using System.Windows;

namespace MathCore.WPF.Commands;

public class TestCommand() : LambdaCommand(p => MessageBox.Show(p?.ToString() ?? "null", "TestCommand"));