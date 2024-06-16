using System.IO;
using System.Text;

namespace MathCore.WPF.WindowTest;

public class LambdaTextWriter(Action<string> Writer) : TextWriter
{
    public override Encoding Encoding { get; }

    public override void Write(char value) => Write($"{value}");

    public override void Write(string? value) => Writer(value);
}