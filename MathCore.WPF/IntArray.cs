using System.Windows.Markup;

namespace MathCore.WPF;

[MarkupExtensionReturnType(typeof(int[]))]
public class IntArray(string Data) : MarkupExtension
{
    public string Data { get; set; } = Data;

    public IntArray() : this(string.Empty) { }

    public override object ProvideValue(IServiceProvider Services)
    {
        var data   = Data.AsStringPtr().Split(';', ' ');
        var values = new List<int>();
        foreach (var str in data)
            if(str.TryParseInt32() is { } value)
                values.Add(value);
        return values.ToArray();
    }
}